using System;
using System.Threading;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors.Temperature;
using Netduino.Foundation.Relays;
using Netduino.Foundation.Sensors;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Generators;
using Netduino.Foundation.Displays;
using Microsoft.SPOT.Net.NetworkInformation;
using System.Net;
using Maple;
using System.IO;

namespace FoodDehydrator3000
{
    public class App
    {
        // peripherals
        protected AnalogTemperature _tempSensor = null;
        protected SoftPwm _heaterRelayPwm = null;
        protected Relay _fanRelay = null;
        protected PushButton _button = null;
        protected SerialLCD _display = null;

        // controllers
        protected DehydratorController _dehydrator = null;

        // vars
        protected NetworkInterface[] _interfaces;
        protected float _currentTemp;
        MapleServer server;

        public App()
        {
            // LCD
            _display = new SerialLCD(new TextDisplayConfig() { Width = 20, Height = 4 });
            _display.Clear();
            Debug.Print("Display up.");
            _display.WriteLine("Display up!", 0);

            // Analog Temp Sensor. Setup to notify at half a degree changes
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A3,
                AnalogTemperature.KnownSensorType.LM35, temperatureChangeNotificationThreshold: 0.5F);
            _tempSensor.TemperatureChanged += (object sender, SensorFloatEventArgs e) => {
                UpdateTemp(e.CurrentValue);
            };
            // display our initial temp
            UpdateTemp(_tempSensor.Temperature);
            Debug.Print("TempSensor up.");
            _display.WriteLine("Temp Sensor up!", 1);

            // Heater driven by Software PWM
            _heaterRelayPwm = new SoftPwm(N.Pins.GPIO_PIN_D3, 0.5f, 1.0f / 30.0f);
            Debug.Print("Heater PWM up.");
            _display.WriteLine("Heater PWM up!", 0);

            // Fan Relay
            _fanRelay = new Relay(N.Pins.GPIO_PIN_D2);
            Debug.Print("Fan up.");
            _display.WriteLine("Fan up!", 1);


            // Button
            _button = new PushButton(N.Pins.GPIO_PIN_D8, Netduino.Foundation.CircuitTerminationType.CommonGround, 100);
            //_button = new PushButton((H.Cpu.Pin)0x15, Netduino.Foundation.CircuitTerminationType.Floating);
            Debug.Print("Button up.");
            _display.WriteLine("Button up!", 0);


            Debug.Print("Peripherals up");
            _display.WriteLine("All systems up!", 1);


            _dehydrator = new DehydratorController(_tempSensor, _heaterRelayPwm, _fanRelay, _display);
            //_button.Clicked += (s,e) => { TogglePower(); };
            _button.LongPressClicked += (s, e) => { TogglePower(); };

            RequestHandler handler = new RequestHandler();
            handler.TurnOff += Handler_TurnOff;
            handler.TurnOn += Handler_TurnOn;
            handler.GetStatus += Handler_GetStatus;

            server = new MapleServer();
            server.AddHandler(handler);
        }

        protected void HandleTempChanged(object sender, Netduino.Foundation.Sensors.SensorFloatEventArgs e)
        {
            _currentTemp = e.CurrentValue;
            UpdateTemp(e.CurrentValue);
        }

        protected void UpdateTemp(float temp)
        {
            _display.WriteLine("Temp: " + temp.ToString("N1") + (char)223 + "C", 1);
        }

        protected void TogglePower()
        {
            if (_dehydrator.Running)
            {
                Debug.Print("PowerButtonClicked, _running == true, turning off.");
                _display.WriteLine("Power OFF.", 0);
                _dehydrator.TurnOff(45);
            }
            else
            {
                Debug.Print("PowerButtonClicked, _running == false, turning on.");
                _dehydrator.TurnOn(50); // set to 35C to start
            }

        }

        public void Run()
        {
            bool networkInit = Netduino.Foundation.Network.Initializer.InitializeNetwork("http://google.com");

            if (networkInit)
            {
                server.Start();
                Debug.Print("Maple server started.");
            }
        }

        public void Stop()
        {
            server.Stop();
            Debug.Print("Maple server stopped.");
        }

        private float Handler_GetStatus()
        {
            if (_dehydrator.Running)
            {
                return _currentTemp;
            }
            else
            {
                return -1;
            }
        }

        private void Handler_TurnOn(int targetTemp)
        {
            _dehydrator.TurnOn(targetTemp);
        }

        private void Handler_TurnOff(int coolDownDelay)
        {
            _dehydrator.TurnOff(coolDownDelay);
        }

        protected void MakeWebRequest(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Debug.Print("this is what we got from " + url + ": " + result);
            }
        }

        protected bool InitializeNetwork()
        {
            if (Microsoft.SPOT.Hardware.SystemInfo.SystemID.SKU == 3)
            {
                Debug.Print("Wireless tests run only on Device");
                return false;
            }

            Debug.Print("Getting all the network interfaces.");
            _interfaces = NetworkInterface.GetAllNetworkInterfaces();

            // debug output
            ListNetworkInterfaces();

            // loop through each network interface
            foreach (var net in _interfaces)
            {
                // debug out
                ListNetworkInfo(net);

                switch (net.NetworkInterfaceType)
                {
                    case (NetworkInterfaceType.Ethernet):
                        Debug.Print("Found Ethernet Interface");
                        break;
                    case (NetworkInterfaceType.Wireless80211):
                        Debug.Print("Found 802.11 WiFi Interface");
                        break;
                    case (NetworkInterfaceType.Unknown):
                        Debug.Print("Found Unknown Interface");
                        break;
                }

                // check for an IP address, try to get one if it's empty
                return CheckIPAddress(net);
            }

            // if we got here, should be false.
            return false;
        }

        protected void ListNetworkInterfaces()
        {
            foreach (var net in _interfaces)
            {
                switch (net.NetworkInterfaceType)
                {
                    case (NetworkInterfaceType.Ethernet):
                        Debug.Print("Found Ethernet Interface");
                        break;
                    case (NetworkInterfaceType.Wireless80211):
                        Debug.Print("Found 802.11 WiFi Interface");
                        break;
                    case (NetworkInterfaceType.Unknown):
                        Debug.Print("Found Unknown Interface");
                        break;
                }
            }
        }

        protected void ListNetworkInfo(NetworkInterface net)
        {
            try
            {
                Debug.Print("MAC Address: " + BytesToHexString(net.PhysicalAddress));
                Debug.Print("DHCP enabled: " + net.IsDhcpEnabled.ToString());
                Debug.Print("Dynamic DNS enabled: " + net.IsDynamicDnsEnabled.ToString());
                Debug.Print("IP Address: " + net.IPAddress.ToString());
                Debug.Print("Subnet Mask: " + net.SubnetMask.ToString());
                Debug.Print("Gateway: " + net.GatewayAddress.ToString());

                if (net is Wireless80211)
                {
                    var wifi = net as Wireless80211;
                    Debug.Print("SSID:" + wifi.Ssid.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.Print("ListNetworkInfo exception:  " + e.Message);
            }

        }

        protected bool CheckIPAddress(NetworkInterface net)
        {
            int timeout = 10000; // timeout, in milliseconds to wait for an IP. 10,000 = 10 seconds

            // check to see if the IP address is empty (0.0.0.0). IPAddress.Any is 0.0.0.0.
            if (net.IPAddress == IPAddress.Any.ToString())
            {
                Debug.Print("No IP Address");

                if (net.IsDhcpEnabled)
                {
                    Debug.Print("DHCP is enabled, attempting to get an IP Address");

                    // ask for an IP address from DHCP [note this is a static, not sure which network interface it would act on]
                    int sleepInterval = 10;
                    int maxIntervalCount = timeout / sleepInterval;
                    int count = 0;
                    while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any && count < maxIntervalCount)
                    {
                        Debug.Print("Sleep while obtaining an IP");
                        Thread.Sleep(10);
                        count++;
                    };

                    // if we got here, we either timed out or got an address, so let's find out.
                    if (net.IPAddress == IPAddress.Any.ToString())
                    {
                        Debug.Print("Failed to get an IP Address in the alotted time.");
                        return false;
                    }

                    Debug.Print("Got IP Address: " + net.IPAddress.ToString());
                    return true;

                    //NOTE: this does not work, even though it's on the actual network device. [shrug]
                    // try to renew the DHCP lease and get a new IP Address
                    //net.RenewDhcpLease ();
                    //while (net.IPAddress == "0.0.0.0") {
                    //    Thread.Sleep (10);
                    //}

                }
                else
                {
                    Debug.Print("DHCP is not enabled, and no IP address is configured, bailing out.");
                    return false;
                }
            }
            else
            {
                Debug.Print("Already had IP Address: " + net.IPAddress.ToString());
                return true;
            }

        }

        private static string BytesToHexString(byte[] bytes)
        {
            string hexString = string.Empty;

            // Create a character array for hexadecimal conversion.
            const string hexChars = "0123456789ABCDEF";

            // Loop through the bytes.
            for (byte b = 0; b < bytes.Length; b++)
            {
                if (b > 0)
                    hexString += "-";

                // Grab the top 4 bits and append the hex equivalent to the return string.        
                hexString += hexChars[bytes[b] >> 4];

                // Mask off the upper 4 bits to get the rest of it.
                hexString += hexChars[bytes[b] & 0x0F];
            }

            return hexString;
        }
    }
}
