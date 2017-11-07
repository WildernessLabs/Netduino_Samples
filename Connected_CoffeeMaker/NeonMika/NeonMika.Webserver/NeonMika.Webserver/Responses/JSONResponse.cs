using System;
using System.Text;
using System.Net.Sockets;
using Microsoft.SPOT;
using FastloadMedia.NETMF.Http;

namespace NeonMika.Webserver.Responses
{
    public class JSONResponse : Response
    {
        /// <summary>
        /// This class knows, HOW to send back the data to the client
        /// Write your own JSONResponseMethod, create a JSONResponse with JSONResponse(url,JSONResponseMethod), and add this response to your webserver instance
        /// </summary>
        public JSONResponse(string url, JSONResponseMethod method)
            : base(url)
        {
            this._ResponseMethod = method;
            _JSONArray = new JsonArray();
        }

        private JSONResponseMethod _ResponseMethod;
        private JsonArray _JSONArray;

        /// <summary>
        /// Execute this to check if SendResponse should be executed
        /// </summary>
        /// <param name="e">The request that should be handled</param>
        /// <returns>True if URL refers to this method, otherwise false (false = SendRequest should not be executed) </returns>
        public override bool ConditionsCheckAndDataFill(Request e)
        {
            _JSONArray.Clear();
            if (e.URL == this.URL)
                _ResponseMethod(e, _JSONArray);
            else
                return false;
            return true;
        }

        /// <summary>
        /// Sends JSON to client
        /// </summary>
        /// <param name="e">Request that should be handled</param>
        /// <returns>True if 200_OK was sent, otherwise false</returns>
        public override bool SendResponse(Request e)
        {
            String jsonResponse = String.Empty;

            jsonResponse = _JSONArray.ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(jsonResponse);

            int byteCount = bytes.Length;

            try
            {
                Send200_OK("application/json", byteCount, e.Client);
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
