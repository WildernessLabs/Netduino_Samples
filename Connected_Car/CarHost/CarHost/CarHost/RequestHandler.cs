using Microsoft.SPOT;
using Maple;

namespace CarHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public event EventHandler Stop = delegate { };
        public event EventHandler TurnLeft = delegate { };
        public event EventHandler TurnRight = delegate { };
        public event EventHandler MoveForward = delegate { };
        public event EventHandler MoveBackward = delegate { };

        public RequestHandler() { }

        public void postStop()
        {
            Stop(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postTurnLeft()
        {
            TurnLeft(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postTurnRight()
        {
            TurnRight(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postMoveForward()
        {
            MoveForward(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postMoveBackward()
        {
            MoveBackward(this, EventArgs.Empty);
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
