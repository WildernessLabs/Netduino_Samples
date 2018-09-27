using Maple;
using Microsoft.SPOT;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Network;
using Netduino.Foundation.RTCs;
using N = SecretLabs.NETMF.Hardware.Netduino;
using System;
using System.Collections;
using System.Threading;

namespace PlantHost
{
    public class App
    {
        public static ArrayList HumidityLogs;

        protected Timer timer = null;
        protected TimerCallback timerCallback = null;

        protected DS3231 _rtc;
        protected RgbPwmLed _rgbPwmLed;
        protected MapleServer _server;
        protected HumiditySensorController _humiditySensor;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();

            HumidityLogs = new ArrayList();
        }

        protected void InitializePeripherals()
        {
            _rtc = new DS3231(0x68, 100);
            //_rtc.CurrentDateTime = new DateTime(2018, 9, 26, 11, 39, 0);

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
            handler.GetPlantHumidity += OnGetPlantHumidity;

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        public void Run()
        {
            Initializer.InitializeNetwork();
            Initializer.NetworkConnected += InitializerNetworkConnected;

            timerCallback = new TimerCallback(TimerInterrupt);
            timer = new Timer(timerCallback, null, TimeSpan.FromTicks(0), new TimeSpan(0,1,0));
            Thread.Sleep(Timeout.Infinite);
        }

        void InitializerNetworkConnected(object sender, EventArgs e)
        {
            _server.Start("PlantHost", Initializer.CurrentNetworkInterface.IPAddress);
            _rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
        }

        void TimerInterrupt(object state)
        {
            HumidityLogs.Add(new HumidityLog()
            {
                Date = _rtc.CurrentDateTime.ToString("hh:mm tt dd/MMM/yyyy"), 
                Humidity = (int) _humiditySensor.Read()
            });

            Thread _animationThread = new Thread(() =>
            {
                _rgbPwmLed.StartBlink(Netduino.Foundation.Color.Blue);
                Thread.Sleep(1000);
                _rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
            });
            _animationThread.Start();
        }

        private void OnGetPlantHumidity(object sender, EventArgs e)
        {
            Thread _animationThread = new Thread(() =>
            {
                _rgbPwmLed.StartBlink(Netduino.Foundation.Color.Orange);
                Thread.Sleep(1000);
                _rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
            });
            _animationThread.Start();
        }
    }
}