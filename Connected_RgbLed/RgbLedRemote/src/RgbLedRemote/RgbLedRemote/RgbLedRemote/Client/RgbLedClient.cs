using System.Threading.Tasks;

namespace RgbLedRemote
{
    public class RgbLedClient : MapleClient
    {
        public async Task<bool> TurnOnAsync(ServerItem server)
        {
            return (await SendCommandAsync("TurnOn", server.IpAddress));
        }

        public async Task<bool> TurnOffAsync(ServerItem server)
        {
            return (await SendCommandAsync("TurnOff", server.IpAddress));
        }

        public async Task<bool> PulseAsync(ServerItem server)
        {
            return (await SendCommandAsync("StartPulse", server.IpAddress));
        }

        public async Task<bool> BlinkAsync(ServerItem server)
        {
            return (await SendCommandAsync("StartBlink", server.IpAddress));
        }

        public async Task<bool> CycleColorsAsync(ServerItem server)
        {
            return (await SendCommandAsync("StartRunningColors", server.IpAddress));
        }
    }
}