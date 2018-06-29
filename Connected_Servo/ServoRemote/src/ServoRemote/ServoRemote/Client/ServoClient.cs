using System.Threading.Tasks;

namespace ServoRemote
{
    public class ServoClient : MapleClient
    {
        public async Task<bool> RotateToAsync(ServerItem server, int degrees)
        {
            return (await SendCommandAsync("RotateTo?targetAngle="+degrees, server.IpAddress));
        }

        public async Task<bool> StartSweepingAsync(ServerItem server)
        {
            return (await SendCommandAsync("StartSweeping", server.IpAddress));
        }

        public async Task<bool> StopSweepingAsync(ServerItem server)
        {
            return (await SendCommandAsync("StopSweeping", server.IpAddress));
        }
    }
}
