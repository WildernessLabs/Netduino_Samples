using Maple;
using Microsoft.SPOT;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Network;
using Netduino.Foundation.RTCs;
using SLH = SecretLabs.NETMF.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using MSH = Microsoft.SPOT.Hardware;
using System;

namespace PlantHost
{
    public class App
    {
        protected DS3231 _rtc;
        protected RgbPwmLed _rgbPwmLed;
        protected MapleServer _server;
        protected HumiditySensorController _humiditySensor;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {
            _rtc = new DS3231(0x68, 100);

            _rgbPwmLed = new RgbPwmLed
            (
                N.PWMChannels.PWM_PIN_D6,
                N.PWMChannels.PWM_PIN_D5,
                N.PWMChannels.PWM_PIN_D3,
                1.05f,
                1.5f,
                1.5f,
                false
            );

            _humiditySensor = new HumiditySensorController
            (
                N.Pins.GPIO_PIN_A0,
                N.Pins.GPIO_PIN_D7
            );

            _rgbPwmLed.SetColor(Netduino.Foundation.Color.Red);
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
            _rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);

            Debug.Print("Date now - " + _rtc.CurrentDateTime);
            Debug.Print("Soil humidity - " + _humiditySensor.Read());
        }
    }
}