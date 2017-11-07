using System;
using Microsoft.SPOT;
using NeonMika.Webserver.Responses;
using System.Text;
using System.IO;
using System.Reflection;

namespace NeonMika.Webserver.Responses.ComplexResponses
{
    /// <summary>
    /// If the root directory is requested, this infopage will be shown
    /// </summary>
    public class IndexResponse : Response
    {
        /// <summary>
        /// Page on which indexPage should be displayed
        /// </summary>
        public IndexResponse(string indexPage)
            : base(indexPage)
        { }

        /// <summary>
        /// Execute this to check if SendResponse shoul be executed
        /// </summary>
        /// <param name="e">The request that should be handled</param>
        /// <returns>True if URL refers to this method, otherwise false (false = SendRequest should not be exicuted) </returns>
        public override bool ConditionsCheckAndDataFill(Request e)
        {
            if (e.URL == "")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sends infotext to client
        /// </summary>
        /// <param name="e">The request which should be handled</param>
        /// <returns>True if 200_OK was sent, otherwise false</returns>
        public override bool SendResponse(Request e)
        {
            Debug.Print(Debug.GC(true).ToString());
            string index = Properties.Resources.GetString(Properties.Resources.StringResources.indexHTML);       
            Send200_OK("text/html", index.Length, e.Client);
            SendData(e.Client,Encoding.UTF8.GetBytes(index));
            index = null;
            return true;
        }
    }
}
