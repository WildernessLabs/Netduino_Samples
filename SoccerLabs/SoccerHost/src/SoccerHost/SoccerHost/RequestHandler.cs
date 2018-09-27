using Maple;
using Microsoft.SPOT;

namespace SoccerHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public event EventHandler Connect = delegate { };
        public event EventHandler KickA = delegate { };
        public event EventHandler KickB = delegate { };

        public void postConnect()
        {
            Connect(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postKickA()
        {
            KickA(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postKickB()
        {
            KickB(this, EventArgs.Empty);
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
