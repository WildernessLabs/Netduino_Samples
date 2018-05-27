using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RgbLedRemote
{
    public class ApiHelper
    {
        public ApiHelper() { }

        async public Task<bool> Connect()
        {
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 0, 3, 0);
            client.BaseAddress = new Uri("http://" + App.HostAddress + "/");

            try
            {
                var response = await client.GetAsync("TurnOn");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return false;
            }
        }

        public async Task<bool> SendCommand(string command)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + App.HostAddress + "/");
            client.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                var response = await client.GetAsync(command);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return false;
            }
        }
    }
}
