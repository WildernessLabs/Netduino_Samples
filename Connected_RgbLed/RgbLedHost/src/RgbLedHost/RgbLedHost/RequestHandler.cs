using Maple;

namespace RgbLedHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public RequestHandler() { }

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

        private void StatusResponse()
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = 200;
            Send();
        }
    }
}
