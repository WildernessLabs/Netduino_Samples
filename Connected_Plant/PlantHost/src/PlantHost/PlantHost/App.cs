using Maple;
using Microsoft.SPOT;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Network;
using SecretLabs.NETMF.Hardware.Netduino;

namespace PlantHost
{
    public class App
    {
        RgbPwmLed rgbPwmLed;
        protected MapleServer _server;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            rgbPwmLed = new RgbPwmLed
            (
                PWMChannels.PWM_PIN_D6,
                PWMChannels.PWM_PIN_D5,
                PWMChannels.PWM_PIN_D3,
                1.05f,
                1.5f,
                1.5f,
                false
            );

            rgbPwmLed.SetColor(Netduino.Foundation.Color.Red);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        public void Run()
        {
            Initializer.InitializeNetwork();
            Initializer.NetworkConnected += InitializerNetworkConnected;
        }

        void InitializerNetworkConnected(object sender, EventArgs e)
        {
            _server.Start("PlantHost", Initializer.CurrentNetworkInterface.IPAddress);
            rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
        }
    }
}