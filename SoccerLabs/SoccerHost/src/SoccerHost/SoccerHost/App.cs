using Maple;
using Microsoft.SPOT;
using Netduino.Foundation.Network;
using Netduino.Foundation.Servos;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace SoccerHost
{
    public class App
    {
        protected MapleServer _server;
        protected PlayerController _playerTeamA;
        protected PlayerController _playerTeamB;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            var servoA = new Servo(N.PWMChannels.PWM_PIN_D3, NamedServoConfigs.Ideal180Servo);
            _playerTeamA = new PlayerController(servoA);

            var servoB = new Servo(N.PWMChannels.PWM_PIN_D11, NamedServoConfigs.Ideal180Servo);
            _playerTeamB = new PlayerController(servoB);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();

            handler.Connect += (s, e) => 
            {
                _playerTeamA.Salute();
                _playerTeamB.Salute();
            };

            handler.KickA += (s, e) => { _playerTeamA.Kick(); };
            handler.KickB += (s, e) => { _playerTeamB.Kick(); };

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        public void Run()
        {
            Initializer.InitializeNetwork();
            Initializer.NetworkConnected += InitializerNetworkConnected;
        }

        private void InitializerNetworkConnected(object sender, EventArgs e)
        {
            _playerTeamA.NetworkConnected();
            _playerTeamB.NetworkConnected();

            _server.Start("SoccerHost01", Initializer.CurrentNetworkInterface.IPAddress);
        }
    }
}