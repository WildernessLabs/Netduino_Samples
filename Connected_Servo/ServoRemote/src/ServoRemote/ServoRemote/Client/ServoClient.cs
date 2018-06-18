using System.Threading.Tasks;

namespace ServoRemote
{
    public class ServoClient : MapleClient
    {
        public async Task<bool> RotateToAsync(ServerItem server, int degrees)
        {
            return (await SendCommandAsync("RotateTo?targetAngle="+degrees, server.IpAddress));
        }

        public async Task<bool> StartCyclingAsync(ServerItem server)
        {
            return (await SendCommandAsync("StartCycling", server.IpAddress));
        }

        public async Task<bool> StopCyclingAsync(ServerItem server)
        {
            return (await SendCommandAsync("StopCycling", server.IpAddress));
        }
    }
}
