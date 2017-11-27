using System;
using Microsoft.SPOT;
using Maple;
using System.Net;

namespace ApplianceHost
{
    public class RequestHandler : RequestHandlerBase
    {
        private static bool _isPowerOn;

        public RequestHandler(HttpListenerContext context) : base(context)
        {
        }

        public void getStatus()
        {
            this.Context.Response.ContentType = "application/text";
            this.Context.Response.StatusCode = 200;
            WriteToOutputStream(_isPowerOn.ToString());
        }

        public void getTurnOn()
        {
            TogglePower(true);
            this.Context.Response.StatusCode = 200;
            this.Context.Response.Close();
        }

        public void getTurnOff()
        {
            TogglePower(false);
            this.Context.Response.StatusCode = 200;
            this.Context.Response.Close();
        }

        private void TogglePower(bool val)
        {
            _isPowerOn = val;
            Ports.ONBOARD_LED.Write(val);
            Ports.GPIO_PIN_D1.Write(val);
        }
    }
}
