using System;
using Microsoft.SPOT;
using Maple;
using System.Net;
using System.Collections;

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
            StatusResponse();
        }

        public void postTurnOn()
        {
            TogglePower(true);
            StatusResponse();
        }

        public void postTurnOff()
        {
            TogglePower(false);
            StatusResponse();
        }

        private void TogglePower(bool val)
        {
            _isPowerOn = val;
            Ports.ONBOARD_LED.Write(val);
            Ports.GPIO_PIN_D1.Write(val);
        }

        private void StatusResponse()
        {
            this.Context.Response.ContentType = "application/json";
            this.Context.Response.StatusCode = 200;
            Hashtable result = new Hashtable { { "isPowerOn", _isPowerOn.ToString().ToLower() } };
            this.Send(result);
        }
    }
}
