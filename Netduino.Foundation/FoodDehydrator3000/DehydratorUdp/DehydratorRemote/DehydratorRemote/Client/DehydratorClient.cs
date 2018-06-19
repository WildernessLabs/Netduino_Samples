using System.Threading.Tasks;

namespace DehydratorRemote
{
    public class DehydratorClient : MapleClient
    {
        public async Task<bool> StatusAsync(ServerItem server)
        {
            return (await GetStatusAsync("status", server.IpAddress));
        }

        public async Task<bool> TurnOnAsync(ServerItem server, int targetTemp)
        {
            return (await TogglePowerAsync("turnon?targetTemp=" + targetTemp, server.IpAddress));
        }

        public async Task<bool> TurnOffAsync(ServerItem server)
        {
            return (await TogglePowerAsync("turnoff?coolDownDelay=5", server.IpAddress));
        }
    }
}
