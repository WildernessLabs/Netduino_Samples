using Maple;
using Microsoft.SPOT;
using Netduino.Foundation.Network;
using Netduino.Foundation.Servos;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace SoccerHost
{
    public class App
    {
        static int _blinkDuration = 100;
        protected MapleServer _server;

        protected ServoController _servoTeamA;
        protected ServoController _servoTeamB;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            var servoA = new Servo(N.PWMChannels.PWM_PIN_D3, NamedServoConfigs.Ideal180Servo);
            _servoTeamA = new ServoController(servoA);

            var servoB = new Servo(N.PWMChannels.PWM_PIN_D11, NamedServoConfigs.Ideal180Servo);
            _servoTeamB = new ServoController(servoB);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();

            handler.ThrowKickA += (s, e) => { _servoTeamA.ThrowKick(); };
            handler.ThrowKickB += (s, e) => { _servoTeamB.ThrowKick(); };

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
            _servoTeamA.NetworkConnected();
            _servoTeamB.NetworkConnected();

            _server.Start("SoccerHost01", Initializer.CurrentNetworkInterface.IPAddress);
        }
    }
}