using System;
using Microsoft.SPOT;
using System.Threading;
using Netduino.Foundation.Network;
using Maple;
using Netduino.Foundation.LEDs;
using System.Collections;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.RTCs;

namespace PlantHost
{
    public class App
    {
        public static ArrayList HumidityLogs;

        protected Timer _timer = null;
        protected TimerCallback _timerCallback = null;

        protected DS3231 _rtc;
        protected RgbPwmLed _rgbPwmLed;
        protected MapleServer _server;
        protected HumiditySensorController _humiditySensor;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();

            HumidityLogs = new ArrayList();
            HumidityLogs = MicroSDController.Load();
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
            handler.GetPlantHumidity += OnGetPlantHumidity;

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        public void Run()
        {
            Initializer.InitializeNetwork();
            Initializer.NetworkConnected += OnNetworkConnected;
        }

        private void OnNetworkConnected(object sender, EventArgs e)
        {
            _timerCallback = new TimerCallback(OnTimerInterrupt);
            _timer = new Timer(_timerCallback, null, TimeSpan.FromTicks(0), new TimeSpan(0, 1, 0));

            _server.Start("PlantHost", Initializer.CurrentNetworkInterface.IPAddress);
            _rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
        }

        private void OnTimerInterrupt(object state)
        {
            HumidityLogs.Add(new HumidityLog()
            {
                Date = _rtc.CurrentDateTime.ToString("hh:mm tt dd/MMM/yyyy"), 
                Humidity = (int)_humiditySensor.Read()
            });

            MicroSDController.Save(HumidityLogs);

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