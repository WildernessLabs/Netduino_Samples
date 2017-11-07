using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace NeonMika.Webserver
{
    /// <summary>
    /// Contains information about a request
    /// </summary>
    public class Request : IDisposable
    {
        /// <summary>
        /// Socket that sent the request
        /// </summary>
        public Socket Client { get; private set; }

        protected string _method;
        protected string _url;
        protected Hashtable _getArguments = new Hashtable();
        protected Hashtable _headers = new Hashtable();

        /// <summary>
        /// All header lines
        /// </summary>
        public Hashtable Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        /// <summary>
        /// Hashtable with all GET key-value pa in it
        /// </summary>
        public Hashtable GetArguments
        {
            get { return _getArguments; }
            private set { _getArguments = value; }
        }

        /// <summary>
        /// HTTP verb (Request method)
        /// </summary>
        public string Method
        {
            get { return _method; }
            private set { _method = value; }
        }

        /// <summary>
        /// URL of request without GET values
        /// </summary>
        public string URL
        {
            get { return _url; }
            private set { _url = value; }
        }

        /// <summary>
        /// Creates request
        /// </summary>
        /// <param name="Data">Input from network</param>
        /// <param name="client">Socket that sent the request</param>
        public Request(char[] header, Socket client)
        {
            this.Client = client;
            ProcessHeader(header);
        }
       
        /// <summary>
        /// Fills the Request with the header values
        /// </summary>
        /// <param name="data">Input from network</param>
        private void ProcessHeader(char[] data)
        {
            bool replace = false;

            for (int i = 0; i < data.Length-3; i++)
            {
                replace = false;

                switch (data[i].ToString()+data[i+1]+data[i+2])
                {
                    case "%5C":
                        data[i] = '\\';
                        data[i+1] = '\0';
                        data[i+2] = '\0';
                        replace = true;
                        break;

                    case "%2F":
                        data[i] = '/';
                        data[i+1] = '\0';
                        data[i+2] = '\0';
                        replace = true;
                        break;                        	
                }

                if(replace)
                for (int x = i + 3; x < data.Length; x++)
                    if (data[x] != '\0')
                    {
                        data[x - 2] = data[x];
                        data[x] = '\0';
                    }
            }

            string content = new string(data);
            string[] lines = content.Split('\n');

            // Parse the first line of the request: "GET /path/ HTTP/1.1"
            string[] firstLineSplit = lines[0].Split(' ');
            _method = firstLineSplit[0];
            string[] path = firstLineSplit[1].Split('?');
            _url = path[0].Substring(1); // Substring to ignore the leading '/'

            _getArguments.Clear();
            if (path.Length > 1)
                ProcessGETParameters(path[1]);

            Headers = NeonMika.Util.Converter.ToHashtable(lines, ": ", 1);
        }

        /// <summary>
        /// Generated Key-Value-Hashtable for GET-Parameters
        /// </summary>
        /// <param name="value"></param>
        private void ProcessGETParameters(string parameters)
        {
            _getArguments = new Hashtable();
            string[] urlArguments = parameters.Split('&');

            _getArguments = NeonMika.Util.Converter.ToHashtable(urlArguments, "=");
        }

        #region IDisposable Members

        public void Dispose()
        {
            if(_headers != null)
                _headers.Clear();
            
            if(_getArguments != null)
                _getArguments.Clear();
        }

        #endregion
    }
}