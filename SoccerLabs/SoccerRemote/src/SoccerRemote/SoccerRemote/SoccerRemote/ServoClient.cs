using System.Threading.Tasks;
using Maple;

namespace SoccerRemote
{
    public class ServoClient : MapleClient
    {
        public async Task<bool> ConnectAsync(ServerItem server)
        {
            return (await SendCommandAsync(ApiConstants.Connect.ToString(), server.IpAddress));
        }

        public async Task<bool> ThrowKickAAsync(ServerItem server)
        {
            return (await SendCommandAsync(ApiConstants.ThrowKickA.ToString(), server.IpAddress));
        }

        public async Task<bool> ThrowKickBAsync(ServerItem server)
        {
            return (await SendCommandAsync(ApiConstants.ThrowKickB.ToString(), server.IpAddress));
        }
    }
}
