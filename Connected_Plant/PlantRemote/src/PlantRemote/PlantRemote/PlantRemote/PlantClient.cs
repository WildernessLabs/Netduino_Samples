using System;
using System.Net.Http;
using System.Threading.Tasks;
using Maple;
using Newtonsoft.Json.Linq;

namespace PlantRemote
{
    public class PlantClient : MapleClient
    {
        public async Task<int> GetHumidityAsync(ServerItem server)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://" + server.IpAddress + "/"),
                Timeout = TimeSpan.FromSeconds(ListenTimeout)
            };

            try
            {
                var response = await client.GetAsync("PlantHumidity");
                if (response.IsSuccessStatusCode)
                {
                    var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return (result["humidity"].Value<int>());
                }
                else
                {
                    throw new InvalidOperationException("Could not connect to device");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}