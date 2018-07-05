using Maple;
using Microsoft.SPOT;

namespace SoccerHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public event EventHandler ThrowKickA = delegate { };
        public event EventHandler ThrowKickB = delegate { };

        public RequestHandler() { }

        public void postThrowKickA()
        {
            ThrowKickA(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postThrowKickB()
        {
            ThrowKickB(this, EventArgs.Empty);
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
