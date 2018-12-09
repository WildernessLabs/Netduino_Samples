using Microsoft.SPOT;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors.Distance;
using Netduino.Foundation.Motors;
using Maple;
using Netduino.Foundation.Network;

namespace CarHost
{
    public class App
    {
        protected HCSR04 _HCSR04;
        protected HBridgeMotor _motor1;
        protected HBridgeMotor _motor2;
        protected MapleServer _mapleServer;
        protected CarController _carController;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            _HCSR04 = new HCSR04(N.Pins.GPIO_PIN_D12, N.Pins.GPIO_PIN_D11);
            _HCSR04.DistanceDetected += OnDistanceDetected;

            _motor1 = new HBridgeMotor(N.PWMChannels.PWM_PIN_D3, N.PWMChannels.PWM_PIN_D5, N.Pins.GPIO_PIN_D4);
            _motor2 = new HBridgeMotor(N.PWMChannels.PWM_PIN_D6, N.PWMChannels.PWM_PIN_D10, N.Pins.GPIO_PIN_D7);
            _carController = new CarController(_motor1, _motor2);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();
            handler.Stop += (s, e) => { _carController.Stop(); };
            handler.TurnLeft += (s, e) => { _carController.TurnLeft(); };
            handler.TurnRight += (s, e) => { _carController.TurnRight(); };
            handler.MoveForward += (s, e) => { _carController.MoveForward(); };
            handler.MoveBackward += (s, e) => { _carController.MoveBackward(); };

            _mapleServer = new MapleServer();
            _mapleServer.AddHandler(handler);
        }

        protected void OnDistanceDetected(object sender, DistanceEventArgs e)
        {
            if (e.Distance == -1 || e.Distance > 30)
                _carController.MoveForward();
            else
                _carController.MoveBackward();

            Debug.Print("current distance: " + e.Distance);
        }

        public void Run()
        {
            Initializer.InitializeNetwork();
            Initializer.NetworkConnected += InitializerNetworkConnected;
        }

        protected void InitializerNetworkConnected(object sender, EventArgs e)
        {
            Debug.Print("InitializeNetwork()");

            _mapleServer.Start("CarHost", Initializer.CurrentNetworkInterface.IPAddress);
            _carController.Stop();
        }
    }
}