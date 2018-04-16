using System;
using Microsoft.SPOT;
using Netduino.Foundation;
using Netduino.Foundation.Generators;
using Netduino.Foundation.Sensors.Temperature;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace ChickenCoop.Micro
{
    public class App
    {
        protected SoftPwm _heatLampRelay;
        protected AnalogTemperature _tempSensor;

        TemperatureController _tempController;

        public App()
        {
            // initialize our peripherals
            _heatLampRelay = new SoftPwm(N.Pins.GPIO_PIN_D2, 0, 1 / 60);
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A2, AnalogTemperature.KnownSensorType.TMP36, updateInterval: 5000, temperatureChangeNotificationThreshold = 1.0);

            // initialize our temp controller
            _tempController = new TemperatureController(_heatLampRelay, _tempSensor);
        }

        public void Run()
        {

            _tempController.TargetTemperature = 15.0f; //15C/60F
            _tempController.Run();
        }
    }
}
