using System;
using System.Threading;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors.Temperature;
using Netduino.Foundation.Relays;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Generators;
using Netduino.Foundation.Displays;

namespace FoodDehydrator3000
{
    public class App
    {
        // peripherals
        protected AnalogTemperature _tempSensor = null;
        protected SoftPwm _heaterRelayPwm = null;
        protected Relay _fanRelay = null;
        protected PushButton _button = null;
        protected SerialLCD _display = null;

        // controllers
        protected DehydratorController _dehydrator = null;

        // members
        protected bool _go = false;

        public App()
        {
            Debug.Print("App()");
            // setup all of our peripherals

            // LCD
            _display = new SerialLCD(new TextDisplayConfig() { Width = 20, Height = 4 });
            Debug.Print("Display up.");

            // Analog Temp Sensor
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A4,
                AnalogTemperature.KnownSensorType.LM35, temperatureChangeNotificationThreshold: 0.1F);
            Debug.Print("TempSensor up.");

            // Heater driven by Software PWM
            _heaterRelayPwm = new SoftPwm(N.Pins.GPIO_PIN_D3, 0.5f, 1.0f / 30.0f);
            Debug.Print("Heater PWM up.");

            // Fan Relay
            _fanRelay = new Relay(N.Pins.GPIO_PIN_D2);
            Debug.Print("Fan up.");

            // Button
            _button = new PushButton(N.Pins.GPIO_PIN_D8, Netduino.Foundation.CircuitTerminationType.CommonGround);
            // waiting on the CircuitTerminationType floating fix
            //_button = new PushButton((H.Cpu.Pin)0x15, Netduino.Foundation.CircuitTerminationType.Floating);
            Debug.Print("Button up.");

            Debug.Print("Peripherals up");

            _dehydrator = new DehydratorController(_tempSensor, _heaterRelayPwm, _fanRelay, _display);
            //_dehydrator.TargetTemperature = 50;

            _button.Clicked += (object sender, EventArgs e) => {
                Debug.Print("Power Button Clicked");
                _dehydrator.PowerButtonClicked();
            };
        }

        public void Run()
        {
            _go = true;
        }
    }
}
