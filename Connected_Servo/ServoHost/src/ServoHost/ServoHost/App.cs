using Netduino.Foundation.Network;
using Maple;
using Netduino.Foundation.Servos;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT;

namespace ServoHost
{
    public class App
    {
        protected MapleServer _server;
        protected ServoController _servoController;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            var _servo = new Servo(N.PWMChannels.PWM_PIN_D9, NamedServoConfigs.Ideal180Servo);
            _servoController = new ServoController(_servo);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();

            handler.RotateTo += (s, e) => { _servoController.RotateTo(((ServoEventArgs)e).Angle); }; ;
            handler.StopSweep += (s, e) => { _servoController.StopSweep(); };
            handler.StartSweep += (s, e) => { _servoController.StartSweep(); };

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
            Debug.Print("InitializeNetwork()");

            _server.Start("ServoHost", Initializer.CurrentNetworkInterface.IPAddress);
            _servoController.NetworkConnected();
        }
    }
}