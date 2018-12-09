using Maple;
using System.Threading.Tasks;

namespace CarRemote.Client
{
    public class CarClient : MapleClient
    {
        public async Task<bool> StopAsync(ServerItem server)
        {
            return (await SendCommandAsync(CommandConstants.STOP, server.IpAddress));
        }

        public async Task<bool> TurnLeftAsync(ServerItem server)
        {
            return (await SendCommandAsync(CommandConstants.TURN_LEFT, server.IpAddress));
        }

        public async Task<bool> TurnRightAsync(ServerItem server)
        {
            return (await SendCommandAsync(CommandConstants.TURN_RIGHT, server.IpAddress));
        }

        public async Task<bool> MoveForwardAsync(ServerItem server)
        {
            return (await SendCommandAsync(CommandConstants.MOVE_FORWARD, server.IpAddress));
        }

        public async Task<bool> MoveBackwardAsync(ServerItem server)
        {
            return (await SendCommandAsync(CommandConstants.MOVE_BACKWARD, server.IpAddress));
        }
    }
}
