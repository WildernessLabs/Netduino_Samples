using Microsoft.SPOT;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Motors;
using Maple;
using Netduino.Foundation.Network;

namespace CarHost
{
    public class App
    {
        protected HBridgeMotor motorLeft;
        protected HBridgeMotor motorRight;
        protected MapleServer _mapleServer;
        protected CarController _carController;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            motorLeft = new HBridgeMotor(N.PWMChannels.PWM_PIN_D3, N.PWMChannels.PWM_PIN_D5, N.Pins.GPIO_PIN_D4);
            motorRight = new HBridgeMotor(N.PWMChannels.PWM_PIN_D6, N.PWMChannels.PWM_PIN_D10, N.Pins.GPIO_PIN_D7);
            _carController = new CarController(motorLeft, motorRight);
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