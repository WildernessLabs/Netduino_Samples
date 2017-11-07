using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FastloadMedia.NETMF.Http;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NeonMika.Webserver.Responses;
using NeonMika.XML;
using NeonMika.Util;
using NeonMika.Webserver.Responses.ComplexResponses;
using NeonMika.Webserver.POST;
using System.IO;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NeonMika.Webserver
{
    /// <summary>
    /// XML Expansion methods have to be in this form
    /// </summary>
    /// <param name="e">Access to GET or POST arguments,...</param>
    /// <param name="results">This hashtable gets converted into xml on response</param>       
    public delegate void XMLResponseMethod(Request e, Hashtable results);

    /// <summary>
    /// JSON Expansion methods have to be in this form
    /// </summary>
    /// <param name="e">Access to GET or POST arguments,...</param>
    /// <param name="results">This JsonArray gets converted into JSON on response</param>
    /// <returns>True if URL refers to this method, otherwise false (false = SendRequest should not be executed) </returns>        
    public delegate void JSONResponseMethod(Request e, JsonArray results);
    
    /// <summary>
    /// Main class of NeonMika.Webserver
    /// </summary>
    public class Server
    {
        public int Port { get; private set; }

        private Socket listeningSocket = null;
        private Hashtable responses = new Hashtable();
        private OutputPort led;
        private bool isToggledOn = false;


        /// <summary>
        /// Creates an NeonMika.Webserver instance running in a seperate thread
        /// </summary>
        /// <param name="portNumber">The port to listen for incoming requests</param>
        public Server(OutputPort ledPort, int port = 80, bool DhcpEnable = true, string ipAddress = "", string subnetMask = "", string gatewayAddress = "", string networkName = "NETDUINOPLUS")
        {
            Debug.Print("\n\n---------------------------");
            Debug.Print("THANKS FOR USING NeonMika.Webserver");
            Debug.Print("Version: " + Settings.SERVER_VERSION);
            Debug.Print("---------------------------");

            this.Port = port;
            this.led = ledPort;

            //NetworkSetup(DhcpEnable, ipAddress, subnetMask, gatewayAddress, networkName);
            //StartLedThread(ledPort);
            ResponseListInitialize();
            SocketSetup();

            var webserverThread = new Thread(WaitingForRequest);
            webserverThread.Start();

            Debug.Print("\n\n---------------------------");
            Debug.Print("Webserver is now up and running");
        }

        /// <summary>
        /// Creates the socket that will listen for incoming requests
        /// </summary>
        private void SocketSetup()
        {
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            listeningSocket.Listen(5);
        }

        /// <summary>
        /// Starts a loop that lets the selected led blink all 2 seconds
        /// </summary>
        /// <param name="ledPort"></param>
        private void StartLedThread(OutputPort ledPort)
        {
            led = ledPort;

            Thread t = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        while (true)
                        {
                            ledPort.Write(true);
                            Thread.Sleep(30);
                            ledPort.Write(false);
                            Thread.Sleep(2000);
                        }
                    }
                    ));
            t.Start();
        }

        /// <summary>
        /// Sets the ip adress and the networkname
        /// </summary>
        /// <param name="DhcpEnable">If true, ip will be received from router via DHCP</param>
        /// <param name="ipAddress"></param>
        /// <param name="subnetMask"></param>
        /// <param name="gatewayAddress"></param>
        /// <param name="networkName">Instead of using the ip, this name can be used in the browser to connect to the device</param>
        private void NetworkSetup(bool DhcpEnable, string ipAddress, string subnetMask, string gatewayAddress, string networkName)
        {
            var interf = NetworkInterface.GetAllNetworkInterfaces()[0];

            if (DhcpEnable)
            {
                //Dynamic IP
                interf.EnableDhcp();
                interf.RenewDhcpLease();
            }
            else
            {
                //Static IP
                interf.EnableStaticIP(ipAddress, subnetMask, gatewayAddress);
            }

            NameService nameService = new NameService();
            nameService.AddName(networkName, NameService.NameType.Unique, NameService.MsSuffix.Default);

            Debug.Print("\n\n---------------------------");
            Debug.Print("Network is set up!\nIP: " + interf.IPAddress + " (DHCP: " + interf.IsDhcpEnabled + ")");
            Debug.Print("You can also reach your Netduino with the following network name: " + networkName);
            Debug.Print("---------------------------");
        }

        /// <summary>
        /// Waiting for client to connect.
        /// When bytes were read they get wrapped to a "Reqeust"
        /// </summary>
        private void WaitingForRequest()
        {
            while (true)
            {
                try
                {
                    using (Socket clientSocket = listeningSocket.Accept())
                    {
                        //Wait to get the bytes in the sockets "available buffer"
                        int availableBytes = AwaitAvailableBytes(clientSocket);

                        if (availableBytes > 0)
                        {
                            byte[] buffer = new byte[availableBytes > Settings.MAX_REQUESTSIZE ? Settings.MAX_REQUESTSIZE : availableBytes];
                            byte[] header = FilterHeader(clientSocket, buffer);

                            //reqeust created, checking the response possibilities
                            using (Request tempRequest = new Request(Encoding.UTF8.GetChars(header), clientSocket))
                            {
                                Debug.Print("\n\nClient connected\nURL: " + tempRequest.URL + "\nFinal byte count: " + availableBytes + "\n");

                                if (tempRequest.Method == "POST")
                                {
                                    //POST was incoming, it will be saved to SD card at Settings.POST_TEMP_PATH
                                    // This file can later be handled in a normal response method by using PostFileReader
                                    PostToSdWriter post = new PostToSdWriter(tempRequest);
                                    post.ReceiveAndSaveData();
                                }

                                //Let's check if we have to take some action or if it is a file-response 
                                SendResponse(tempRequest);
                            }

                            try
                            {
                                //Close client, otherwise the browser / client won't work properly
                                clientSocket.Close();
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.ToString());
                            }

                            Debug.Print("Reqeust finished");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        /// <summary>
        /// Reads in the data from the socket and seperates the header from the rest of the request.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="buffer">Will get filled with the incoming data</param>
        /// <returns>The header</returns>
        private byte[] FilterHeader(Socket clientSocket, byte[] buffer)
        {
            byte[] header = new byte[0];
            int readByteCount = clientSocket.Receive(buffer, buffer.Length, SocketFlags.None);

            for (int headerend = 0; headerend < buffer.Length - 3; headerend++)
            {
                if (buffer[headerend] == '\r' && buffer[headerend + 1] == '\n' && buffer[headerend + 2] == '\r' && buffer[headerend + 3] == '\n')
                {
                    header = new byte[headerend + 4];
                    Array.Copy(buffer, 0, header, 0, headerend + 4);
                    break;
                }
            }

            return header;
        }

        /// <summary>
        /// Returns the number of available bytes.
        /// Waits till all bytes from one request are received.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <returns></returns>
        private int AwaitAvailableBytes(Socket clientSocket)
        {
            int availableBytes = 0;
            int newAvBytes;

            do
            {
                //Wait if bytes come in
                Thread.Sleep(15);
                newAvBytes = clientSocket.Available - availableBytes;

                // breaks the "always true loop" if no new bytes got available
                if (newAvBytes == 0)
                    break;

                availableBytes += newAvBytes;
                newAvBytes = 0;
            } while (true); //repeat as long as new bytes were received

            return availableBytes;
        }

        /// <summary>
        /// Checks what Response has to be executed.
        /// It compares the requested page URL with the URL set for the coded responses 
        /// </summary>
        /// <param name="e"></param>
        private void SendResponse(Request e)
        {
            Response response = null;


            if (responses.Contains(e.URL))
                response = (Response)responses[e.URL];
            else
                response = (Response)responses["FileResponse"];


            if (response != null)
            {
                using (response)
                {
                    if (response.ConditionsCheckAndDataFill(e))
                    {
                        if (!response.SendResponse(e))
                            Debug.Print("Sending response failed");
                    }
                    else
                    {
                        response.Send404_NotFound(e.Client);
                    }
                }
            }
        }

        //-------------------------------------------------------------
        //-------------------------------------------------------------
        //---------------Webserver expansion---------------------------
        //-------------------------------------------------------------
        //-------------------------------------------------------------
        //-------------------Basic methods-----------------------------

        /// <summary>
        /// Adds a Response
        /// </summary>
        /// <param name="response">XMLResponse that has to be added</param>
        public void AddResponse(Response response)
        {
            if (!responses.Contains(response.URL))
            {
                responses.Add(response.URL, response);
            }
        }

        /// <summary>
        /// Removes a Response
        /// </summary>
        /// <param name="ResponseName">XMLResponse that has to be deleted</param>
        public void RemoveResponse(String ResponseName)
        {
            if (responses.Contains(ResponseName))
            {
                responses.Remove(ResponseName);
            }
        }

        //-------------------------------------------------------------
        //-------------------------------------------------------------
        //-----------------------EXPAND this methods-------------------

        /// <summary>
        /// Initialize the basic functionalities of the webserver
        /// </summary>
        private void ResponseListInitialize()
        {
            AddResponse(new IndexResponse(""));
            AddResponse(new FileResponse());
            AddResponse(new XMLResponse("echo", new XMLResponseMethod(Echo)));
            AddResponse(new XMLResponse("switchDigitalPin", new XMLResponseMethod(SwitchDigitalPin)));
            AddResponse(new XMLResponse("setDigitalPin", new XMLResponseMethod(SetDigitalPin)));
            AddResponse(new XMLResponse("xmlResponseList", new XMLResponseMethod(ResponseListXML)));
            AddResponse(new JSONResponse("jsonResponseList", new JSONResponseMethod(ResponseListJSON)));
            AddResponse(new XMLResponse("pwm", new XMLResponseMethod(SetPWM)));
            AddResponse(new XMLResponse("getAnalogPinValue", new XMLResponseMethod(GetAnalogPinValue)));
            AddResponse(new XMLResponse("getDigitalPinState", new XMLResponseMethod(GetDigitalPinState)));
            AddResponse(new XMLResponse("multixml", new XMLResponseMethod(MultipleXML)));
            AddResponse(new XMLResponse("upload", new XMLResponseMethod(Upload)));
            AddResponse(new XMLResponse("getAllDigitalPinStates", new XMLResponseMethod(GetAllDigitalPinStates)));
            AddResponse(new XMLResponse("getAllAnalogPinValues", new XMLResponseMethod(GetAllAnalogPinValues)));
            AddResponse(new XMLResponse("getAllPWMValues", new XMLResponseMethod(GetAllPWMValues)));
        }

        //-------------------------------------------------------------
        //---------------------Expansion Methods-----------------------
        //-------------------------------------------------------------
        //----------Look at the echo method for xml example------------

        /// <summary>
        /// Example for webserver expand method
        /// Call via http://servername/echo?value='echovalue'
        /// Submit a 'value' GET parameter
        /// </summary>
        /// <param name="e"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        private void Echo(Request e, Hashtable results)
        {
            if (e.GetArguments.Contains("value") == true)
                results.Add("echo", e.GetArguments["value"]);
            else
                results.Add("ERROR", "No 'value'-parameter transmitted to server");
        }

        /// <summary>
        /// Submit a 'pin' GET parameter to switch an OutputPorts state (on/off)
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private static void SwitchDigitalPin(Request e, Hashtable h)
        {
            if (e.GetArguments.Contains("pin"))
                try
                {
                    int pin = Int32.Parse(e.GetArguments["pin"].ToString());
                    if (pin >= 0 && pin <= 13)
                    {
                        PinManagement.SwitchDigitalPinState(pin);
                        h.Add("pin" + pin, PinManagement.GetDigitalPinState(pin) ? "1" : "0");
                    }
                }
                catch
                {
                    h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterConvertError);
                }
            else
                h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterMissing);
        }

        /// <summary>
        /// Submit a 'pin' (0-13) and a 'state' (true/false) GET parameter to turn on/off OutputPort
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private static void SetDigitalPin(Request e, Hashtable h)
        {
            if (e.GetArguments.Contains("pin"))
                if (e.GetArguments.Contains("state"))
                    try
                    {
                        int pin = Int32.Parse(e.GetArguments["pin"].ToString());
                        if (pin >= 0 && pin <= 13)
                        {
                            try
                            {
                                bool state = (e.GetArguments["state"].ToString() == "true") ? true : false;
                                PinManagement.SetDigitalPinState(pin, state);
                                h.Add("pin" + pin, PinManagement.GetDigitalPinState(pin) ? "1" : "0");
                            }
                            catch
                            {
                                h = XMLResponse.GenerateErrorHashtable("state", ResponseErrorType.ParameterRangeException);
                            }
                        }
                        else
                            h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterRangeException);
                    }
                    catch
                    {
                        h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterConvertError);
                    }
                else
                    h = XMLResponse.GenerateErrorHashtable("state", ResponseErrorType.ParameterMissing);
            else
                h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterMissing);
        }

        /// <summary>
        /// Returns the responses added to the webserver
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private void ResponseListXML(Request e, Hashtable h)
        {
            int i = -1;
            foreach (Object k in responses.Keys)
            {
                i++;
                if (responses[k] as XMLResponse != null)
                {
                    h.Add("methodURL"+i, k.ToString());
                }
            }
        }

        /// <summary>
        /// Returns the responses added to the webserver
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private void ResponseListJSON(Request e, JsonArray j)
        {
            JsonObject o;
            foreach (Object k in responses.Keys)
            {
                if (responses[k] as JSONResponse != null)
                {
                    o = new JsonObject();
                    o.Add("methodURL", k);
                    o.Add("methodInternalName", ((Response)responses[k]).URL);
                    j.Add(o);
                }
            }
        }

        /// <summary>
        /// Submit a 'pin' (5,6,9,10), a period and a duration (0 for off, period-value for 100% on) GET parameter to control PWM
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private void SetPWM(Request e, Hashtable h)
        {
            if (e.GetArguments.Contains("pin"))
            {
                if (e.GetArguments.Contains("period"))
                {
                    if (e.GetArguments.Contains("duration"))
                    {
                        try
                        {
                            int pin = Int32.Parse(e.GetArguments["pin"].ToString());
                            try
                            {
                                uint duration = UInt32.Parse(e.GetArguments["duration"].ToString());
                                try
                                {
                                    uint period = UInt32.Parse(e.GetArguments["period"].ToString());
                                    if (PinManagement.SetPWM(pin, period, duration))
                                        h.Add("success", period + "/" + duration);
                                    else
                                        h = XMLResponse.GenerateErrorHashtable("PWM", ResponseErrorType.InternalValueNotSet);
                                }
                                catch (Exception ex)
                                {
                                    h = XMLResponse.GenerateErrorHashtable("period", ResponseErrorType.ParameterConvertError);
                                    Debug.Print(ex.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                h = XMLResponse.GenerateErrorHashtable("duration", ResponseErrorType.ParameterConvertError);
                                Debug.Print(ex.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterConvertError);
                            Debug.Print(ex.ToString());
                        }
                    }
                    else
                        h = XMLResponse.GenerateErrorHashtable("duration", ResponseErrorType.ParameterMissing);
                }
                else
                    h = XMLResponse.GenerateErrorHashtable("period", ResponseErrorType.ParameterMissing);
            }
            else
                h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterMissing);
        }

        /// <summary>
        /// Submit a 'pin' (0-13) GET parameter. Returns true or false
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private void GetDigitalPinState(Request e, Hashtable h)
        {
            if (e.GetArguments.Contains("pin"))
            {
                try
                {
                    int pin = Int32.Parse(e.GetArguments["pin"].ToString());
                    h.Add("pin" + pin, PinManagement.GetDigitalPinState(pin) ? "1" : "0");
                }
                catch (Exception ex)
                {
                    h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterConvertError);
                    Debug.Print(ex.ToString());
                }
            }
            else
                h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterMissing);
        }

        /// <summary>
        /// Returns the state of all digital pins
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        private void GetAllDigitalPinStates(Request e, Hashtable h)
        {
            try
            {
                for (int i = 0; i < PinManagement.DIGITAL_PIN_COUNT; i++)
                {
                    h.Add("pin" + i, PinManagement.GetDigitalPinState(i) ? "1" : "0");
                }
            }
            catch (Exception ex)
            {
                h = XMLResponse.GenerateErrorHashtable("", ResponseErrorType.InternalOperationError);
                Debug.Print(ex.ToString());
            }
        }

        /// <summary>
        /// Returns the period and the duration of all PWM pins
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        private void GetAllPWMValues(Request e, Hashtable h)
        {
            try
            {
                for (int i = 0; i < PinManagement.PWM_IDs.Length; i++)
                {
                    int id = PinManagement.PWM_IDs[i];
                    h.Add("pin" + PinManagement.PWM_IDs[i] + "_period", PinManagement.GetPWMPeriod(id));
                    h.Add("pin" + PinManagement.PWM_IDs[i] + "_duration", PinManagement.GetPWMDuration(id));
                }
            }
            catch (Exception ex)
            {
                h = XMLResponse.GenerateErrorHashtable("", ResponseErrorType.InternalOperationError);
                Debug.Print(ex.ToString());
            }
        }

        /// <summary>
        /// Returns the state of all analog pins
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        private void GetAllAnalogPinValues(Request e, Hashtable h)
        {
            try
            {
                for (int i = 0; i < PinManagement.ANALOG_PIN_COUNT; i++)
                {
                    h.Add("pin" + i, PinManagement.GetAnalogPinValue(i));
                }
            }
            catch (Exception ex)
            {
                h = XMLResponse.GenerateErrorHashtable("", ResponseErrorType.InternalOperationError);
                Debug.Print(ex.ToString());
            }
        }

        /// <summary>
        /// Submit a 'pin' (0-5) GET parameter. Returns true or false
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private void GetAnalogPinValue(Request e, Hashtable h)
        {
            if (e.GetArguments.Contains("pin"))
            {
                try
                {
                    int pin = Int32.Parse(e.GetArguments["pin"].ToString());
                    h.Add("pin" + pin, PinManagement.GetAnalogPinValue(pin));
                }
                catch (Exception ex)
                {
                    h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterConvertError);
                    Debug.Print(ex.ToString());
                }
            }
            else
                h = XMLResponse.GenerateErrorHashtable("pin", ResponseErrorType.ParameterMissing);
        }

        /// <summary>
        /// Example for the useage of the new XML library
        /// Use the hashtable if you don't need nested XML (like the standard xml responses)
        /// If you need nested XML, use the XMLPair class. The Key-parameter is String.
        /// As value the following types can be used to achieve nesting: XMLPair, XMLPair[] and Hashtable
        /// </summary>
        /// <param name="e"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private void MultipleXML(Request e, Hashtable returnHashtable)
        {
            returnHashtable.Add("UseTheHashtable", "If you don't need nested XML");

            XMLList Phones = new XMLList("Phones");
            Phones.Attributes.Add("ExampleAttribute1", "NeonMika");
            Phones.Attributes.Add("ExampleAttribute2", 1992);
            XMLList BluePhones = new XMLList("BluePhones");
            XMLList BlackPhones = new XMLList("BlackPhones");
            XMLList MokiaRumia = new XMLList("Phone");
            XMLList LangsumTalaxy = new XMLList("Phone");
            MokiaRumia.Add(new XMLPair("Name", "Mokia Rumia"));
            MokiaRumia.Add(new XMLPair("PhoneNumber", 436603541897));
            XMLList WirelessConnections = new XMLList("WirelessConnections");
            WirelessConnections.Add(new XMLPair("WLAN", true));
            WirelessConnections.Add(new XMLPair("Bluetooth", false));
            MokiaRumia.Add(WirelessConnections);
            WirelessConnections.Clear();
            WirelessConnections.Add(new XMLPair("WLAN", false));
            WirelessConnections.Add(new XMLPair("Bluetooth", true));
            LangsumTalaxy.Add(new XMLPair("Name", "Langsum Talaxy"));
            LangsumTalaxy.Add(new XMLPair("PhoneNumber", 436603541122));
            LangsumTalaxy.Add(WirelessConnections);

            Phones.Add(MokiaRumia);
            Phones.Add(LangsumTalaxy);

            returnHashtable.Add("Phones", Phones);
        }

        private void Upload(Request e, Hashtable ret)
        {
            if (e.GetArguments.Contains("path"))
            {
                try
                {
                    string filePath = e.GetArguments["path"].ToString();
                    filePath = filePath.Replace('/', '\\');

                    // Create directory for file destination
                    try
                    {
                        Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf("\\")));
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.ToString());
                        ret.Add("Error", "Could not create directory");
                    }

                    try
                    {
                        // Open write stream
                        FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                        Debug.Print(Debug.GC(true).ToString());
                        Debug.Print(Debug.GC(true).ToString());

                        // Instanziate PostFileReader
                        // We will use this to load the file block by block and save those blocks to another file
                        PostFileReader post = new PostFileReader();
                        Debug.Print(Debug.GC(true).ToString());
                        Debug.Print(Debug.GC(true).ToString());

                        // Here the file copy happens:
                        // 1. Copy full blocks
                        long nrBlocks = post.Length/Settings.FILE_BUFFERSIZE;
                        for (int i = 0; i < nrBlocks; i++)
                        {
                            fs.Write(post.Read(Settings.FILE_BUFFERSIZE), 0, Settings.FILE_BUFFERSIZE);
                            fs.Flush();
                            Debug.GC(true);
                            Debug.GC(true);
                            Debug.Print("Block " + i + " of " + nrBlocks + " written\n");
                        }
                        // 2. Copy last not-full block
                        fs.Write(post.Read((int)(post.Length % Settings.FILE_BUFFERSIZE)), 0, (int)(post.Length % Settings.FILE_BUFFERSIZE));
                        fs.Flush();
                        // File saved, close file writing filestream
                        fs.Close();
                        // Close reading file
                        post.Close();
                        ret.Add("Message","File upload successfull");
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.ToString());
                        ret = XMLResponse.GenerateErrorHashtable("file access", ResponseErrorType.InternalOperationError);
                    }

                }
                catch (Exception ex) { Debug.Print(ex.ToString()); }
            }
            else
                ret = XMLResponse.GenerateErrorHashtable("path", ResponseErrorType.ParameterMissing);
        }
    }
}

namespace System.Diagnostics
{
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}