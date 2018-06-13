using System.Threading.Tasks;

namespace RgbLedRemote
{
    public class RgbLedClient : MapleClient
    {
        public bool IsOn { get; set; } = false;
        public bool IsBlinking { get; set; } = false;
        public bool IsPulsing { get; set; } = false;
        public bool IsCyclingColors { get; set; } = false;

        public RgbLedClient()
        {
        }

        void ResetState ()
        {
            IsOn = false;
            IsBlinking = false;
            IsPulsing = false;
            IsCyclingColors = false;
        }

        public async Task TurnOnAsync(ServerItem server)
        {
            if (await SendCommandAsync("TurnOn", server.IpAddress))
            {
                ResetState();
                IsOn = true;
            }
        }

        public async Task TurnOffAsync(ServerItem server)
        {
            if (await SendCommandAsync("TurnOff", server.IpAddress))
            {
                ResetState();
                IsOn = false;
            }
        }

        public async Task PulseAsync(ServerItem server)
        {
            if(await SendCommandAsync("StartPulse", server.IpAddress))
            {
                ResetState();
                IsPulsing = true;
            }
        }

        public async Task BlinkAsync(ServerItem server)
        {
            if(await SendCommandAsync("StartBlink", server.IpAddress))
            {
                ResetState();
                IsBlinking = true;
            }
        }

        public async Task CycleColorsAsync(ServerItem server)
        {
            if(await SendCommandAsync("StartRunning", server.IpAddress))
            {
                ResetState();
                IsCyclingColors = true;
            }
        }
    }
}