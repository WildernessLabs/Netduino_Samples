using System;
using System.Threading;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors.Temperature;
using Netduino.Foundation.Relays;
using Netduino.Foundation.Sensors.Buttons;

namespace FoodDehydrator3000
{
    public class App
    {
        // peripherals
        protected AnalogTemperature _tempSensor = null;
        // protected Relay _heaterRelay = null;
        protected H.PWM _heaterRelayPwm = null;
        protected Relay _fanRelay = null;
        protected PushButton _button = null;

        // controllers
        protected DehydratorController _dehydrator = null;

        // members
        protected bool _go = false;

        public App()
        {
            // setup all of our peripherals
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A4,
            AnalogTemperature.KnownSensorType.LM35, temperatureChangeNotificationThreshold: 0.1F);
            _heaterRelayPwm = new H.PWM(N.PWMChannels.PWM_PIN_D3, 1.0 / 30, 0.0, false);
            _fanRelay = new Relay(N.Pins.GPIO_PIN_D2);
            _button = new PushButton(N.Pins.ONBOARD_BTN, Netduino.Foundation.CircuitTerminationType.CommonGround);

            _dehydrator = new DehydratorController(_tempSensor, _heaterRelayPwm, _fanRelay);

            _button.Clicked += (object sender, EventArgs e) => {
                _dehydrator.PowerButtonClicked();
            };
        }

        public void Run()
        {
            _go = true;
        }
    }
}
