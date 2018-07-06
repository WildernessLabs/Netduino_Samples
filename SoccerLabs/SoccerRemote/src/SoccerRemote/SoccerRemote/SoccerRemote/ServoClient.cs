using System.Threading.Tasks;
using Maple;

namespace SoccerRemote
{
    public class ServoClient : MapleClient
    {
        public async Task<bool> TurnOnAsync(ServerItem server)
        {
            return (await SendCommandAsync("ThrowKickA", server.IpAddress));
        }

        public async Task<bool> TurnOffAsync(ServerItem server)
        {
            return (await SendCommandAsync("ThrowKickB", server.IpAddress));
        }
    }
}
