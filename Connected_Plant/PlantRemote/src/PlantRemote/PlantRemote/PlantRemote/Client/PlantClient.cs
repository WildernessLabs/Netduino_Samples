using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Maple;
using Newtonsoft.Json;

namespace PlantRemote
{
    public class PlantClient : MapleClient
    {
        public async Task<List<HumidityLog>> GetHumidityAsync(ServerItem server)
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
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<HumidityLog>>(jsonResponse);

                    return result;
                }
                else
                {
                    throw new InvalidOperationException("Could not connect to device");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<HumidityLog>();
            }
        }
    }
}