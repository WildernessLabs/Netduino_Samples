using Maple;

namespace RgbLedHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public RequestHandler() { }

        public void postLightOn()
        {
            LightOn();
            StatusResponse();
        }
        public delegate void OnLightOn();
        public event OnLightOn LightOn = delegate { };

        public void postLightOff()
        {
            LightOff();
            StatusResponse();
        }
        public delegate void OnLightOff();
        public event OnLightOff LightOff = delegate { };

        public void postBlink()
        {
            StartBlink();
            StatusResponse();
        }
        public delegate void OnBlink();
        public event OnBlink StartBlink = delegate { };

        public void postPulse()
        {
            StartPulse();
            StatusResponse();
        }
        public delegate void OnPulse();
        public event OnPulse StartPulse = delegate { };

        public void postRunningColors()
        {
            StartRunningColors();
            StatusResponse();
        }
        public delegate void OnSweepColors();
        public event OnSweepColors StartRunningColors = delegate { };

        private void StatusResponse()
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = 200;
            Send();
        }
    }
}
