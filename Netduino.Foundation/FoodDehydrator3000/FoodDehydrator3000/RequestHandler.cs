using System;
using Microsoft.SPOT;
using Maple;
using System.Net;
using System.Collections;

namespace FoodDehydrator3000
{
    public class RequestHandler : RequestHandlerBase
    {
        public RequestHandler()
        {
        }

        /******************************* STATUS *******************************/

        public void getStatus()
        {
            try
            {
                var currentTemp = GetStatus();
                Hashtable result = new Hashtable { { "currentTemp", currentTemp } };
                StatusResponse(ContentTypes.Application_Json, 200, result);
            }
            catch(Exception ex)
            {
                StatusResponse(ContentTypes.Application_Text, 500, ex.Message);
            }
        }
        public delegate float StatusHandler();
        public event StatusHandler GetStatus = delegate { return 0; };

        /******************************* TURN ON *******************************/

        public void postTurnOn()
        {
            try
            {
                int targetTemp = 0;

                if (this.Body == null || this.Body["targetTemp"] == null)
                {
                    StatusResponse(ContentTypes.Application_Text, 400, "targetTemp is required");
                    return;
                }
                try
                {
                    targetTemp = int.Parse(this.Body["targetTemp"].ToString());
                }
                catch(Exception ex)
                {
                    StatusResponse(ContentTypes.Application_Text, 400, "Invalid targetTemp value");
                }
                
                TurnOn(targetTemp);
                StatusResponse(200);
            }
            catch (Exception ex)
            {
                StatusResponse(ContentTypes.Application_Text, 500, ex.Message);
            }

        }
        public delegate void TurnOnHandler(int targetTemp);
        public event TurnOnHandler TurnOn = delegate { };

        /******************************* TURN OFF *******************************/

        public void postTurnOff()
        {
            try
            {
                int coolDownDelay = 0;

                if (this.Body == null || this.Body["coolDownDelay"] == null)
                {
                    StatusResponse(ContentTypes.Application_Text, 400, "coolDownDelay is required");
                    return;
                }
                try
                {
                    coolDownDelay = int.Parse(this.Body["coolDownDelay"].ToString());
                }
                catch (Exception ex)
                {
                    StatusResponse(ContentTypes.Application_Text, 400, "Invalid coolDownDelay value");
                }

                TurnOff(coolDownDelay);
                StatusResponse(200);
            }
            catch (Exception ex)
            {
                StatusResponse(ContentTypes.Application_Text, 500, ex.Message);
            }

        }
        public delegate void TurnOffHandler(int coolDownDelay);
        public event TurnOffHandler TurnOff = delegate { };

        /**********************************************************************/

        private void StatusResponse(int statusCode)
        {
            StatusResponse(null, statusCode, null);
        }
        private void StatusResponse(string contentType, int statusCode)
        {
            StatusResponse(contentType, statusCode, null);
        }
        private void StatusResponse(string contentType, int statusCode, object payload)
        {
            this.Context.Response.ContentType = contentType;
            this.Context.Response.StatusCode = statusCode;
            this.Send(payload);
        }
    }
}
