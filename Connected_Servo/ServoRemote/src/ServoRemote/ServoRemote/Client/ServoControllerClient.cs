using System.Threading.Tasks;
using Maple;

namespace ServoRemote
{
    public class ServoControllerClient : MapleClient
    {
        public async Task<bool> RotateToAsync(ServerItem server, int degrees)
        {
            return (await SendCommandAsync("RotateTo?targetAngle="+degrees, server.IpAddress));
        }

        public async Task<bool> StartSweepAsync(ServerItem server)
        {
            return (await SendCommandAsync("StartSweep", server.IpAddress));
        }

        public async Task<bool> StopSweepAsync(ServerItem server)
        {
            return (await SendCommandAsync("StopSweep", server.IpAddress));
        }
    }
}
