using Maple;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CarRemote.Client
{
    public class CarClient : MapleClient
    {
        public async Task MoveAsync(ServerItem server)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://" + server.IpAddress + "/"),
                Timeout = TimeSpan.FromSeconds(ListenTimeout)
            };

            try
            {
                var response = await client.GetAsync("PlantHumidity", HttpCompletionOption.ResponseContentRead);
                if (response.IsSuccessStatusCode)
                {
                    //string jsonResponse = await response.Content.ReadAsStringAsync();
                    //var result = JsonConvert.DeserializeObject<List<HumidityLog>>(jsonResponse);

                    return;
                }
                else
                {
                    throw new InvalidOperationException("Could not connect to device");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return;
            }
        }
    }
}
