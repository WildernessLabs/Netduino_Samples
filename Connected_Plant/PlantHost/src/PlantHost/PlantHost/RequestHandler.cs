using Microsoft.SPOT;
using Maple;

namespace PlantHost
{
    public class RequestHandler : RequestHandlerBase
    {
        public event EventHandler GetPlantHumidity = delegate { };

        public void getPlantHumidity()
        {
            GetPlantHumidity(this, EventArgs.Empty);
            StatusResponse();
        }

        protected void StatusResponse()
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = 200;
            string json = Json.NETMF.JsonSerializer.SerializeObject(App.HumidityLogs);
            Send(json);
        }
    }
}