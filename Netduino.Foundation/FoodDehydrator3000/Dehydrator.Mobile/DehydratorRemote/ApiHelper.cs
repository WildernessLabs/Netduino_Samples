using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DehydratorRemote
{
    public class ApiHelper
    {
        private string _hostAddress;
        private string _apiBase = "";

        public ApiHelper(string hostAddress)
        {
            if (hostAddress == "127.0.0.1")
            {
                _hostAddress = "localhost:5000";
            }
            else
            {
                _hostAddress = hostAddress;
            }
        }

        async public Task<bool> CheckStatus()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + _hostAddress + "/" + _apiBase);
            var response = await client.GetAsync("status");
            if (response.IsSuccessStatusCode)
            {
                var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                return result["isRunning"].Value<bool>();
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

            try
            {
                client.BaseAddress = new Uri("http://" + _hostAddress + "/" + _apiBase);
                var response = await client.GetAsync("status");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        async public Task<bool> TurnOn(int targetTemp)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + _hostAddress + "/" + _apiBase);

            var response = await client.PostAsync("turnon?targetTemp=" + targetTemp, null);
            return response.IsSuccessStatusCode;
        }

        async public Task<bool> TurnOff()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + _hostAddress + "/" + _apiBase);

            var response = await client.PostAsync("turnoff?coolDownDelay=5", null);
            return response.IsSuccessStatusCode;
        }

    }
}
