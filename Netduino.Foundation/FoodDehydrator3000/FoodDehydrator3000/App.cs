using System;
using System.Threading;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors.Temperature;
using Netduino.Foundation.Relays;
using Netduino.Foundation.Sensors;
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
            _display.Clear();
            Debug.Print("Display up.");
            _display.WriteLine("Display up!", 0);

            // Analog Temp Sensor. Setup to notify at half a degree changes
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A3,
                AnalogTemperature.KnownSensorType.LM35, temperatureChangeNotificationThreshold: 0.5F);
            _tempSensor.TemperatureChanged += (object sender, SensorFloatEventArgs e) => {
                UpdateTemp(e.CurrentValue);
            };
            // display our initial temp
            UpdateTemp(_tempSensor.Temperature);
            Debug.Print("TempSensor up.");
            _display.WriteLine("Temp Sensor up!", 1);

            // Heater driven by Software PWM
            _heaterRelayPwm = new SoftPwm(N.Pins.GPIO_PIN_D3, 0.5f, 1.0f / 30.0f);
            Debug.Print("Heater PWM up.");
            _display.WriteLine("Heater PWM up!", 0);

            // Fan Relay
            _fanRelay = new Relay(N.Pins.GPIO_PIN_D2);
            Debug.Print("Fan up.");
            _display.WriteLine("Fan up!", 1);


            // Button
            _button = new PushButton(N.Pins.GPIO_PIN_D8, Netduino.Foundation.CircuitTerminationType.CommonGround, 100);
            // waiting on the CircuitTerminationType floating fix
            //_button = new PushButton((H.Cpu.Pin)0x15, Netduino.Foundation.CircuitTerminationType.Floating);
            Debug.Print("Button up.");
            _display.WriteLine("Button up!", 0);


            Debug.Print("Peripherals up");
            _display.WriteLine("All systems up!", 1);


            _dehydrator = new DehydratorController(_tempSensor, _heaterRelayPwm, _fanRelay, _display);
            _button.Clicked += (s,e) => { TogglePower(); };
        }

        protected void HandleTempChanged(object sender, Netduino.Foundation.Sensors.SensorFloatEventArgs e)
        {
            UpdateTemp(e.CurrentValue);
        }

        protected void UpdateTemp(float temp)
        {
            _display.WriteLine("Temp: " + temp.ToString("N1") + (char)223 + "C", 1);
        }

        protected void TogglePower()
        {
            if (_dehydrator.Running)
            {
                Debug.Print("PowerButtonClicked, _running == true, turning off.");
                _display.WriteLine("Power OFF.", 0);
                _dehydrator.TurnOff();
            }
            else
            {
                Debug.Print("PowerButtonClicked, _running == false, turning on.");
                _dehydrator.TurnOn(35); // set to 35C to start
            }

        }

        public void Run()
        {
            _go = true;
        }
    }
}
