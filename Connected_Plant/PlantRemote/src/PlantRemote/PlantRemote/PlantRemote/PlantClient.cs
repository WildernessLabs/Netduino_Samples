using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Maple;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlantRemote
{
    public class PlantClient : MapleClient
    {
        public async Task<List<HumidityModel>> GetHumidityAsync(ServerItem server)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://" + server.IpAddress + "/"),
                Timeout = TimeSpan.FromSeconds(ListenTimeout)
            };

            try
            {
                var response = await client.GetAsync("PlantHumidity", HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<object>(jsonResponse);
                    var result2 = JsonConvert.DeserializeObject<List<HumidityModel>>(result.ToString());

                    return result2;
                }
                else
                {
                    throw new InvalidOperationException("Could not connect to device");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<HumidityModel>();
            }
        }
    }
}