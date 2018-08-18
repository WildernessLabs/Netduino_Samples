using System.Threading.Tasks;
using Maple;

namespace PlantRemote
{
    public class PlantClient : MapleClient
    {
        public async Task<bool> TurnOnAsync(ServerItem server)
        {
            return (await SendCommandAsync("TurnOn", server.IpAddress));
        }
    }
}
