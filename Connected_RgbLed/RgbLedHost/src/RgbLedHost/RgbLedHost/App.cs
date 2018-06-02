using Maple;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace RgbLedHost
{
    public class App
    {
        static int BlinkRate = 100;
        protected MapleServer server;
        protected RgbLedController controller;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            controller = new RgbLedController();
        }

        protected void InitializeWebServer()
        {
            RequestHandler handler = new RequestHandler();

            handler.LightOn += OnLightOn;
            handler.LightOff += OnLightOff;
            handler.StartBlink += OnBlink;
            handler.StartPulse += OnPulse;
            handler.StartRunningColors += OnRunningColors;

            server = new MapleServer();
            server.AddHandler(handler);
        }

        void OnLightOn()
        {
            controller.LightOn();
        }

        void OnLightOff()
        {
            controller.LightOff();
        }

        void OnBlink()
        {
            controller.Blink();
        }

        void OnPulse()
        {
            controller.Pulse();
        }

        void OnRunningColors()
        {
            controller.RunningColors();
        }

        public void Run()
        {
            Netduino.Foundation.Network.Initializer.NetworkConnected += InitializerNetworkConnected;
            Netduino.Foundation.Network.Initializer.InitializeNetwork();

            var led = new OutputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.ONBOARD_LED, false);
            Debug.Print("InitializeNetwork()");

            while (true)
            {
                led.Write(true);
                Thread.Sleep(BlinkRate);
                led.Write(false);
                Thread.Sleep(BlinkRate);
            }
        }

        void InitializerNetworkConnected(object sender, EventArgs e)
        {
            Debug.Print("Connected! (do work)");
            BlinkRate = 1000;

            server.Start();
            controller.NetworkConnected();
        }
    }
}
