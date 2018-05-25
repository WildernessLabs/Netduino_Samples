using Maple;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation.LEDs;
using System.Collections;
using System.Threading;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace RgbLedHost
{
    public class App
    {
        static int BlinkRate = 100;
        protected LedStatus ledStatus;
        protected RgbPwmLed rgbPwmLed;
        protected MapleServer server;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            rgbPwmLed = new RgbPwmLed
            (
                N.PWMChannels.PWM_PIN_D11,
                N.PWMChannels.PWM_PIN_D10,
                N.PWMChannels.PWM_PIN_D9,
                1.05f,
                1.5f,
                1.5f,
                false
            );

            ledStatus = LedStatus.On;
            rgbPwmLed.StartBlink(Netduino.Foundation.Color.Red);
        }

        protected void InitializeWebServer()
        {
            // configure our web server
            RequestHandler handler = new RequestHandler();
            handler.TurnOn += HandlerTurnOn;
            handler.TurnOff += HandlerTurnOff;
            handler.StartBlink += HandlerStartBlink;
            handler.StartPulse += HandlerStartPulse;
            handler.StartRunningColors += HandlerStartRunningColors;

            server = new MapleServer();
            server.AddHandler(handler);
        }

        void HandlerTurnOn()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.SetColor(Netduino.Foundation.Color.Red);
        }

        void HandlerTurnOff()
        {
            rgbPwmLed.Stop();
        }

        void HandlerStartBlink()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.StartBlink(Netduino.Foundation.Color.Green);
        }

        void HandlerStartPulse()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.StartPulse(Netduino.Foundation.Color.Blue);
        }

        void HandlerStartRunningColors()
        {
            var arrayColors = new ArrayList();
            for (int i = 0; i < 360; i = i + 5)
            {
                var hue = ((double)i / 360F);
                arrayColors.Add(Netduino.Foundation.Color.FromHsba(((double)i / 360F), 1, 1));
            }

            int[] intervals = new int[arrayColors.Count];
            for (int i = 0; i < intervals.Length; i++)
            {
                intervals[i] = 1000;
            }

            rgbPwmLed.Stop();
            rgbPwmLed.StartRunningColors(arrayColors, intervals);
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
            rgbPwmLed.Stop();
            rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
        }

        public void Stop()
        {

        }
    }
}