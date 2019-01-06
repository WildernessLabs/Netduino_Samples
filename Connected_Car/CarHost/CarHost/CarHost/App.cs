using Microsoft.SPOT;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Motors;
using Maple;
using Netduino.Foundation.Network;
using Netduino.Foundation.LEDs;

namespace CarHost
{
    public class App
    {
        protected PwmLed _redLed;
        protected PwmLed _greenLed;
        protected HBridgeMotor _motorLeft;
        protected HBridgeMotor _motorRight;
        protected MapleServer _mapleServer;
        protected CarController _carController;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            _redLed = new PwmLed(N.PWMChannels.PWM_PIN_D11, TypicalForwardVoltage.Red);
            _greenLed = new PwmLed(N.PWMChannels.PWM_PIN_D10, TypicalForwardVoltage.Green);
            _motorLeft = new HBridgeMotor(N.PWMChannels.PWM_PIN_D3, N.PWMChannels.PWM_PIN_D5, N.Pins.GPIO_PIN_D4);
            _motorRight = new HBridgeMotor(N.PWMChannels.PWM_PIN_D6, N.PWMChannels.PWM_PIN_D9, N.Pins.GPIO_PIN_D7);
            _carController = new CarController(_motorLeft, _motorRight);

            _redLed.StartBlink();
            _greenLed.SetBrightness(0f);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();
            handler.Stop += OnStop;
            handler.TurnLeft += OnTurnLeft;
            handler.TurnRight += OnTurnRight;
            handler.MoveForward += OnMoveForward;
            handler.MoveBackward += OnMoveBackward;

            _mapleServer = new MapleServer();
            _mapleServer.AddHandler(handler);
        }

        private void OnStop(object sender, EventArgs e)
        {
            _carController.MoveBackward();
            _redLed.Stop();
        }

        private void OnTurnLeft(object sender, EventArgs e)
        {
            _carController.MoveBackward();
            _redLed.StartBlink();
        }

        private void OnTurnRight(object sender, EventArgs e)
        {
            _carController.MoveBackward();
            _redLed.StartBlink();
        }

        private void OnMoveForward(object sender, EventArgs e)
        {
            _carController.MoveForward();
            _redLed.StartPulse();
        }

        private void OnMoveBackward(object sender, EventArgs e)
        {
            _carController.MoveBackward();
            _redLed.StartPulse();
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

            _redLed.Stop();
            _greenLed.SetBrightness(0.75f);
            _carController.Stop();
        }
    }
}