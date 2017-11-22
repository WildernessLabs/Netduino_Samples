using System;
using Microsoft.SPOT;
using Maple;
using System.Net;

namespace ApplianceHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public RequestHandler(HttpListenerContext context) : base(context)
        {
        }

        public void getStatus()
        {
            var ledStatus = Ports.Led.Read();
            this.Context.Response.ContentType = "application/text";
            this.Context.Response.StatusCode = 200;
            WriteToOutputStream(ledStatus.ToString());
        }

        public void getTurnOn()
        {
            Ports.Led.Write(true);
            this.Context.Response.StatusCode = 200;
            this.Context.Response.Close();
        }

        public void getTurnOff()
        {
            Ports.Led.Write(false);
            this.Context.Response.StatusCode = 200;
            this.Context.Response.Close();
        }
    }
}
