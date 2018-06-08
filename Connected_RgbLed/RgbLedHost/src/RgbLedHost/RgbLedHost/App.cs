using Maple;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation.LEDs;
using System.Threading;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace RgbLedHost
{
    public class App
    {
        static int _blinkDuration = 100;
        protected MapleServer _server;
        protected RgbLedController _rgbController;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            var rgbPwmLed = new RgbPwmLed
            (
                N.PWMChannels.PWM_PIN_D11,
                N.PWMChannels.PWM_PIN_D10,
                N.PWMChannels.PWM_PIN_D9,
                1.05f,
                1.5f,
                1.5f,
                false
            );

            _rgbController = new RgbLedController(rgbPwmLed);
        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();

            handler.TurnOn += (objects, args) => { _rgbController.TurnOn(); };
            handler.TurnOff += (objects, args) => { _rgbController.TurnOff(); };
            handler.StartBlink += (objects, args) => { _rgbController.StartBlink(); };
            handler.StartPulse += (objects, args) => { _rgbController.StartPulse(); };
            handler.StartRunningColors += (objects, args) => { _rgbController.StartRunningColors(); };

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        public void Run()
        {
            Netduino.Foundation.Network.Initializer.NetworkConnected += InitializerNetworkConnected;
            Netduino.Foundation.Network.Initializer.InitializeNetwork();

            var led = new OutputPort(N.Pins.ONBOARD_LED, false);
            Debug.Print("InitializeNetwork()");

            while (true)
            {
                led.Write(true);
                Thread.Sleep(_blinkDuration);
                led.Write(false);
                Thread.Sleep(_blinkDuration);
            }
        }

        void InitializerNetworkConnected(object sender, EventArgs e)
        {
            Debug.Print("Connected! (do work)");
            _blinkDuration = 1000;

            _server.Start();
            _rgbController.NetworkConnected();
        }
    }
}
