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
            var _servo = new Servo(N.PWMChannels.PWM_PIN_D9, NamedServoConfigs.Ideal270Servo);
            _servoController = new ServoController(_servo);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();

            handler.GoToAngleZero += (s, e) => { _servoController.GoToAngleZero(); };
            handler.StartCycling += (s, e) => { _servoController.StartCycling(); };
            handler.StopCycling += (s, e) => { _servoController.StopCycling(); };

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        public void Run()
        {
            Initializer.InitializeNetwork();

            Debug.Print("InitializeNetwork()");

            while (Initializer.CurrentNetworkInterface == null) { }

            _server.Start("ServoHost", Initializer.CurrentNetworkInterface.IPAddress);
            _servoController.NetworkConnected();
        }
    }
}
