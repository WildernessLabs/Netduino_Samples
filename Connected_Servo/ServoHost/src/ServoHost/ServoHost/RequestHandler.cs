using Maple;
using Microsoft.SPOT;

namespace ServoHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public event EventHandler GoToAngleZero = delegate { };
        public event EventHandler StartCycling = delegate { };
        public event EventHandler StopCycling = delegate { };

        public RequestHandler() { }

        public void postGoToAngleZero()
        {
            GoToAngleZero(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postStartCycling()
        {
            StartCycling(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postStopCyclin()
        {
            StopCycling(this, EventArgs.Empty);
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