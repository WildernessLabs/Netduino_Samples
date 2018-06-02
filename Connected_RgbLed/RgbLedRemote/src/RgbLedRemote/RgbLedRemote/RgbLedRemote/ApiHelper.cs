using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RgbLedRemote
{
    public class ApiHelper
    {
        public ApiHelper() { }

        public async Task<bool> SendCommand(string command)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + App.HostAddress + "/");
            client.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                var response = await client.PostAsync(command, null);
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
