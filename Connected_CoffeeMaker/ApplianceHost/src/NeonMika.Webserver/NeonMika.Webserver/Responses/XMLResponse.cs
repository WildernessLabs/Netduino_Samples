using System;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using Microsoft.SPOT;
using NeonMika.XML;

namespace NeonMika.Webserver.Responses
{
    /// <summary>
    /// This class knows, HOW to send back the data to the client
    /// Write your own XMLResponseMethod, create a XMLResponse with XMLResponse(url,XMlResponseMethod), and add this response to your webserver instance
    /// </summary>
    public class XMLResponse : Response
    {
        public XMLResponse(string url, XMLResponseMethod method)
            : base(url)
        {
            this._ResponseMethod = method;
            _Pairs = new Hashtable();
        }

        private XMLResponseMethod _ResponseMethod;
        private Hashtable _Pairs;

        /// <summary>
        /// Unifies the possible error messages
        /// </summary>
        /// <param name="parameter">Name of the parameter on which the error took place</param>
        /// <param name="ret">Error type</param>
        /// <returns>Hashtable that has to be returned to the client</returns>
        public static Hashtable GenerateErrorHashtable(String parameter, ResponseErrorType ret)
        {
            Hashtable h = new Hashtable();

            switch (ret)
            {
                case ResponseErrorType.ParameterConvertError:
                    h.Add("error","Following parameter could not be converted: " + parameter + " to the right format (int, string, ...).");
                    break;
                case ResponseErrorType.ParameterMissing:
                    h.Add("error","Following parameter was not submitted: " + parameter + ". Please include it in your URL");
                    break;
                case ResponseErrorType.InternalValueNotSet:
                    h.Add("error","An internal error accured. Following value could not be set: " + parameter + ". Please check the requested method's source code");
                    break;
                case ResponseErrorType.ParameterRangeException:
                    h.Add("error","Following parameter was out of range: " + parameter + ".");
                    break;
                case ResponseErrorType.InternalOperationError:
                    h.Add("error", "An internal error accured. Following code part threw the error: " + parameter + ". Please check the requested method's source code");
                    break;
            }

            return h;
        }

        /// <summary>
        /// Execute this to check if SendResponse shoul be executed
        /// </summary>
        /// <param name="e">The request that should be handled</param>
        /// <returns>True if URL refers to this method, otherwise false (false = SendRequest should not be exicuted) </returns>
        public override bool ConditionsCheckAndDataFill(Request e)
        {
            _Pairs.Clear();
            if (e.URL == this.URL)
                _ResponseMethod(e, _Pairs);
            else
                return false;
            return true;
        }

        /// <summary>
        /// Sends XML to client
        /// </summary>
        /// <param name="e">The request which should be handled</param>
        /// <returns>True if 200_OK was sent, otherwise false</returns>
        public override bool SendResponse(Request e)
        {
            //Some self promotion :P
            String xml = "<!-- This xml response was created by NeonMika.Webserver -->";
            xml += "<!-- Hello " + e.Client.RemoteEndPoint + ", hope you have a nice day :) -->";

            xml += "<Response>";

            foreach ( object h in _Pairs.Keys )
                xml += "<" + h + ">" + _Pairs[h].ToString() + "</" + h + ">";

            xml += "</Response>";

            byte[] bytes = Encoding.UTF8.GetBytes(xml);

            int byteCount = bytes.Length;

            try
            {
                Send200_OK("text/xml", byteCount, e.Client);
                SendData(e.Client, bytes);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                return false;
            }

            return true;
        }

        
    }
}
