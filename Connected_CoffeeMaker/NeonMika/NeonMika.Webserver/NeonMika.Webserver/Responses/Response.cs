using System;
using System.Text;
using System.Net.Sockets;
using Microsoft.SPOT;

namespace NeonMika.Webserver.Responses
{
    /// <summary>
    /// Abstract class for responses
    /// Contains basic operations for sending data to the client
    /// </summary>
    abstract public class Response : IDisposable
    {
        private string _url;

        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Creates response to send back to client
        /// </summary>
        /// <param name="beforeFileSearchMethods">Webserver expand methods</param>
        public Response(string Name)
        {
            this._url = Name;
        }

        /// <summary>
        /// Creates header for 200 OK response
        /// </summary>
        /// <param name="MimeType">MIME type of response</param>
        /// <param name="ContentLength">Byte count of response body
        /// <param name="Client">The Socket connected with the client</param>
        protected void Send200_OK(string MimeType, int ContentLength, Socket Client)
        {
            /*
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append("HTTP/1.0 200 OK\r\n");
            headerBuilder.Append("Content-Type: ");
            headerBuilder.Append(MimeType);
            headerBuilder.Append("; charset=utf-8\r\n");
            headerBuilder.Append("Content-Length: ");
            headerBuilder.Append(ContentLength.ToString());
            headerBuilder.Append("\r\n");
            headerBuilder.Append("Connection: close\r\n\r\n");
             * */

            String header;
            if(ContentLength>0)
                header = "HTTP/1.0 200 OK\r\n" + "Content-Type: " + MimeType + "; charset=utf-8\r\n" + "Content-Length: " + ContentLength.ToString() + "\r\n" + "Connection: close\r\n\r\n";
            else
                header = "HTTP/1.0 200 OK\r\n" + "Content-Type: " + MimeType + "; charset=utf-8\r\n" + "Connection: close\r\n\r\n";

            try
            {
                Client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message.ToString());
                return;
            }
        }

        /// <summary>
        /// Sends a 404 Not Found response
        /// </summary>
        public void Send404_NotFound(Socket Client)
        {
            string header = "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n<html><body><head><title>NeonMika.Webserver is sorry</title></head><h1>NeonMika.Webserver is sorry!</h1><h2>The file or webmethod you were looking for was not found :/</h2></body></html>";
            if (Client != null)
                Client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
            Debug.Print("Sent 404 Not Found");
        }

        /// <summary>
        /// Sends data to the client
        /// </summary>
        /// <param name="client">Socket connected with the client</param>
        /// <param name="data">Byte-array to be transmitted</param>
        /// <returns>Bytes that were sent</returns>
        protected int SendData(Socket client, byte[] data)
        {
            int ret = 0;
            try
            {
                if (SocketConnected(client))
                    ret = client.Send(data, data.Length, SocketFlags.None);
                else
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Error on sending data to client / Closing Client");
                try
                {
                    client.Close();
                }
                catch (Exception ex2)
                {
                }
            }

            return ret;
        }

        /// <summary>
        /// Converts fileending into mime-type
        /// </summary>
        /// <param name="Filename">File name or complete file path</param>
        /// <returns>MIME type</returns>
        protected string MimeType(string Filename)
        {
            string result = "text/html";
            int dot = Filename.LastIndexOf('.');

            string ext = (dot >= 0) ? Filename.Substring(dot + 1) : "";
            switch (ext.ToLower())
            {
                case "txt":
                    result = "text/plain";
                    break;
                case "htm":
                case "html":
                    result = "text/html";
                    break;
                case "js":
                    result = "text/javascript";
                    break;
                case "css":
                    result = "text/css";
                    break;
                case "xml":
                case "xsl":
                    result = "text/xml";
                    break;
                case "jpg":
                case "jpeg":
                    result = "image/jpeg";
                    break;
                case "gif":
                    result = "image/gif";
                    break;
                case "png":
                    result = "image/png";
                    break;
                case "ico":
                    result = "x-icon";
                    break;
                case "mid":
                    result = "audio/mid";
                    break;
                default:
                    result = "application/octet-stream";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Checks if socket is still connected
        /// </summary>
        /// <param name="s">Socket that should be checked</param>
        /// <returns>True on still connect</returns>
        protected bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 & part2)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Override this method to implement a response logic.
        /// </summary>
        /// <returns>True if Response was sent, false if not</returns>
        abstract public bool SendResponse(Request e);

        /// <summary>
        /// Override this, check the URL and process data if needed
        /// </summary>
        /// <returns>True if SendResponse should be sent, false if not</returns>
        abstract public bool ConditionsCheckAndDataFill(Request e);

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
