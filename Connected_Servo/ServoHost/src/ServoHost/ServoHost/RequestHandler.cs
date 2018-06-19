using Maple;
using Microsoft.SPOT;
using System;

namespace ServoHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public event EventHandler RotateTo = delegate { };
        public event EventHandler StartCycling = delegate { };
        public event EventHandler StopCycling = delegate { };

        public RequestHandler() { }

        public void postRotateTo()
        {
            try
            {
                int targetAngle = 0;
                var prm = "targetAngle";

                try
                {
                    var temp = this.Body?[prm] ?? this.Form?[prm] ?? this.QueryString?[prm];
                    targetAngle = int.Parse(temp.ToString());
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }

                RotateTo(this, new ServoEventArgs(targetAngle));
                StatusResponse();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }            
        }

        public void postStopCycling()
        {
            StopCycling(this, EventArgs.Empty);
            StatusResponse();
        }

        public void postStartCycling()
        {
            StartCycling(this, EventArgs.Empty);
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