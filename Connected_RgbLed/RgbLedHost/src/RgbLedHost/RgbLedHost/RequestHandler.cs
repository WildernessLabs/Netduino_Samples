using Maple;
using System.Collections;

namespace RgbLedHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public RequestHandler() { }

        public void getStatus()
        {
            var status = GetStatus();
            StatusResponse();
        }
        public delegate LedStatus StatusHandler();
        public event StatusHandler GetStatus = delegate { return LedStatus.Off; };

        public void getTurnOn()
        {
            TurnOn();
            StatusResponse();
        }
        public delegate void TurnOnHandler();
        public event TurnOnHandler TurnOn = delegate { };

        public void getBlink()
        {
            StartBlink();
            StatusResponse();
        }
        public delegate void BlinkHandler();
        public event BlinkHandler StartBlink = delegate { };

        public void getPulse()
        {
            StartPulse();
            StatusResponse();
        }
        public delegate void PulseHandler();
        public event PulseHandler StartPulse = delegate { };

        public void getRunningColors()
        {
            StartRunningColors();
            StatusResponse();
        }
        public delegate void RunningColorsHandler();
        public event RunningColorsHandler StartRunningColors = delegate { };

        public void getTurnOff()
        {
            TurnOff();
            StatusResponse();
        }
        public delegate void TurnOffHandler();
        public event TurnOffHandler TurnOff = delegate { };

        private void StatusResponse()
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = 200;
            //Hashtable result = new Hashtable { { "status", ledStatus.ToString().ToLower() } };
            Send();
        }
    }
}