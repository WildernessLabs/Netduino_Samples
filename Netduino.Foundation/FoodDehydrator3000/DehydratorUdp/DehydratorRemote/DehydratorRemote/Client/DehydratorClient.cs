using System.Threading.Tasks;

namespace DehydratorRemote
{
    public class DehydratorClient : MapleClient
    {
        public async Task<bool> StatusAsync(ServerItem server)
        {
            return (await GetStatusAsync("Status", server.IpAddress));
        }

        public async Task<bool> TurnOnAsync(ServerItem server, int targetTemp)
        {
            return (await TogglePowerAsync("TurnOn?targetTemp=" + targetTemp, server.IpAddress));
        }

        public async Task<bool> TurnOffAsync(ServerItem server)
        {
            return (await TogglePowerAsync("TurnOff", server.IpAddress));
        }
    }
}
