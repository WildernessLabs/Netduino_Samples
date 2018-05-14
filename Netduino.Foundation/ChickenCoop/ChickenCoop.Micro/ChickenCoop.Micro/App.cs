using System;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using Netduino.Foundation;
using Netduino.Foundation.Generators;
using Netduino.Foundation.Sensors.Temperature;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace ChickenCoop.Micro
{
    public class App
    {
        public static App Current = null;

        // peripherals
        protected SoftPwm _heatLampRelay;
        protected AnalogTemperature _tempSensor;

        // controllers
        protected TemperatureController _tempController;

        public AppConfig Config {
            get { return _config; }
        } protected AppConfig _config = null;
        

        public App()
        {
            Current = this;

            // initialize our app configuration
            this._config = new AppConfig();

            // setup network
            InitializeNetworkAndTime();

            // setup peripherals
            InitializePeripherals();

            // initialize our temp controller
            _tempController = new TemperatureController(_heatLampRelay, _tempSensor);
        }

        public void Run()
        {
            _tempController.TargetTemperature = 15.0f; //15C/60F
            _tempController.Run();
        }

        protected void InitializeNetworkAndTime()
        {
            // initialize our network interfaces
            Netduino.Foundation.Network.Initializer.InitializeNetwork();

            // get the current date time from the server
            var dateTime = Netduino.Foundation.NetworkTime.GetNetworkTime((int)App.Current.Config.UtcOffset);

            Debug.Print("Current DateTime: " + dateTime.ToString());

            // save the date time
            H.Utility.SetLocalTime(dateTime);
        }

        protected void InitializePeripherals()
        {
            // initialize our peripherals
            _heatLampRelay = new SoftPwm(N.Pins.GPIO_PIN_D2, 0, 1 / 60);
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A2, AnalogTemperature.KnownSensorType.LM35, updateInterval: 5000, temperatureChangeNotificationThreshold: 1.0f);

        }
    }
}
