using Maple;
using Microsoft.SPOT;

namespace RgbLedHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public event EventHandler TurnOn = delegate { };
        public event EventHandler TurnOff = delegate { };
        public event EventHandler StartBlink = delegate { };
        public event EventHandler StartPulse = delegate { };
        public event EventHandler StartRunningColors = delegate { };

        public RequestHandler() { }

        public void postTurnOn()
        {
            TurnOn(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postTurnOff()
        {
            TurnOff(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postStartBlink()
        {
            StartBlink(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postStartPulse()
        {
            StartPulse(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postStartRunningColors()
        {
            this.StartRunningColors(this, EventArgs.Empty);
            StatusResponse();
        }

        private void StatusResponse()
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = 200;
            Send();
        }
    }
}