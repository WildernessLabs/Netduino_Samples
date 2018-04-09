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
                // if the status is less than 0 then the dehydrator is not running
                var currentTemp = GetStatus();
                bool isRunning = currentTemp >= 0;

                Hashtable result = new Hashtable
                {
                    { "isRunning", isRunning },
                    { "currentTemp", currentTemp >= 0 ? currentTemp : 0 }
                };
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
                var prm = "targetTemp";

                if (this.Body[prm] == null && this.Form[prm] == null && this.QueryString[prm] == null)
                {
                    StatusResponse(ContentTypes.Application_Text, 400, prm + " is required");
                    return;
                }
                try
                {
                    var temp = this.Body[prm] ?? this.Form[prm] ?? this.QueryString[prm];
                    targetTemp = int.Parse(temp.ToString());
                }
                catch(Exception ex)
                {
                    StatusResponse(ContentTypes.Application_Text, 400, "Invalid " + prm + " value");
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
                var prm = "coolDownDelay";
                int coolDownDelay = 0;
                var coolDownDelayValue = this.Body[prm] ?? this.Form[prm] ?? this.QueryString[prm] ?? null;
               
                try
                {
                    if(coolDownDelayValue != null)
                    {
                        coolDownDelay = int.Parse(coolDownDelayValue.ToString());
                    }
                }
                catch (Exception ex)
                {
                    StatusResponse(ContentTypes.Application_Text, 400, "Invalid " + prm + " value");
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
