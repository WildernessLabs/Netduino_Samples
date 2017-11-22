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
            var ledStatus = Ports.ONBOARD_LED.Read();
            this.Context.Response.ContentType = "application/text";
            this.Context.Response.StatusCode = 200;
            WriteToOutputStream(ledStatus.ToString());
        }

        public void getTurnOn()
        {
            var val = true;
            Ports.ONBOARD_LED.Write(val);
            Ports.GPIO_PIN_D1.Write(val);
            this.Context.Response.StatusCode = 200;
            this.Context.Response.Close();
        }

        public void getTurnOff()
        {
            var val = false;
            Ports.ONBOARD_LED.Write(val);
            Ports.GPIO_PIN_D1.Write(val);
            this.Context.Response.StatusCode = 200;
            this.Context.Response.Close();
        }
    }
}
