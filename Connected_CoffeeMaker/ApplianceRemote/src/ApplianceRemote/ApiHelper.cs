using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ApplianceRemote
{
    public class ApiHelper
    {
        private string _hostAddress;
        private string _apiBase = "";

        public ApiHelper(string hostAddress)
        {
            if(hostAddress == "127.0.0.1"){
                _hostAddress = "localhost:5000";
            }
            else{
                _hostAddress = hostAddress;
            }
        }

        async public Task<bool> CheckStatus()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + _hostAddress+ "/" + _apiBase);
            var response = client.GetAsync("status").Result;
            if (response.IsSuccessStatusCode)
            {
                //var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                //return result["isOn"].Value<bool>();
                var result = await response.Content.ReadAsStringAsync();
                return string.Compare(result, "true", true) == 0;
            }
            else
            {
                throw new InvalidOperationException("Could not connect to device");
            }
        }

        async public Task<bool> Connect()
        {
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 0, 3, 0);
            client.BaseAddress = new Uri("http://" + _hostAddress+ "/" + _apiBase);
            try{
                var response = await client.GetAsync("status");
                return response.IsSuccessStatusCode;    
            }
            catch(Exception ex){
                return false;
            }

        }

        async public Task<bool> TurnOn()
        {
            return await PowerCommand("turnon");
        }

        async public Task<bool> TurnOff()
        {
            return await PowerCommand("turnoff");
        }

        async private Task<bool> PowerCommand(string command)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + _hostAddress + "/" + _apiBase);

            var response = await client.GetAsync(command);

            if (response.IsSuccessStatusCode)
            {
                if (command == "turnon")
                {
                    App.ApplianceStatus = ApplianceStatus.On;
                }
                else
                {
                    App.ApplianceStatus = ApplianceStatus.Off;
                }
            }
            return response.IsSuccessStatusCode;
        }
    }
}
