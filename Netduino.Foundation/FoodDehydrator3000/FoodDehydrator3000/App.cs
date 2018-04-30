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
using Microsoft.SPOT.Net.NetworkInformation;
using System.Net;
using Maple;
using System.IO;
using Netduino.Foundation.Displays.TextDisplayMenu;
using Netduino.Foundation.Sensors.Rotary;
using FoodDehydrator3000.Properties;
using System.Text;
using System.Collections;

namespace FoodDehydrator3000
{
    public class App
    {
        // peripherals
        protected AnalogTemperature _tempSensor = null;
        protected SoftPwm _heaterRelayPwm = null;
        protected Relay _fanRelay = null;
        //protected PushButton _button = null;
        protected SerialLCD _display = null;

        RotaryEncoderWithButton _encoder = null;

        // controllers
        protected DehydratorController _dehydrator = null;

        // vars
        protected NetworkInterface[] _interfaces;
        protected float _currentTemp;
        MapleServer server;
        Menu _menu;
        private DateTime _tempUpdated;
        private float _targetTemp;
        private TimeSpan _runTime;

        public App()
        {
            // Rotary Encoder
            _encoder = new RotaryEncoderWithButton(
                N.Pins.GPIO_PIN_D5, N.Pins.GPIO_PIN_D6, N.Pins.GPIO_PIN_D7,
                Netduino.Foundation.CircuitTerminationType.CommonGround);

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

            Debug.Print("Peripherals up");
            _display.WriteLine("All systems up!", 1);

            _menu = new Menu(_display, _encoder, Resources.GetBytes(Resources.BinaryResources.menu));
            _menu.ValueChanged += HandleMenuValueChange;
            _menu.Selected += HandleMenuSelected;

            _dehydrator = new DehydratorController(_tempSensor, _heaterRelayPwm, _fanRelay, _display);

            RequestHandler handler = new RequestHandler();
            handler.TurnOff += Handler_TurnOff;
            handler.TurnOn += Handler_TurnOn;
            handler.GetStatus += Handler_GetStatus;

            server = new MapleServer();
            server.AddHandler(handler);
        }

        private void HandleMenuSelected(object sender, MenuSelectedEventArgs e)
        {
            if(e.Command == "power")
            {
                TogglePower();
            }
        }

        private void HandleMenuValueChange(object sender, ValueChangedEventArgs e)
        {
            if(e.ItemID == "temperature")
            {
                _targetTemp = (float)(double)e.Value; //smh
                _dehydrator.TargetTemperature = _targetTemp;
                _menu.UpdateItemValue("displayTargetTemp", e.Value);
            }
            else if(e.ItemID == "timer")
            {
                _runTime = (TimeSpan)e.Value;
            }
        }

        protected void UpdateTemp(float temp)
        {
            int updateInterval = 5;

            if(_menu != null)
            {
                if(DateTime.Now > _tempUpdated.AddSeconds(updateInterval))
                {
                    Debug.Print("Update display");
                    TimeSpan remainingTime = _dehydrator.RunningTimeLeft;

                    Hashtable values = new Hashtable();
                    values.Add("displayCurrentTemp", temp);
                    values.Add("temperature", _targetTemp);
                    values.Add("displayRemainingTime", PadLeft(remainingTime.Hours.ToString(), '0', 2) + ":" + PadLeft(remainingTime.Minutes.ToString(), '0', 2));
                    _menu.UpdateItemValue(values);
                    _tempUpdated = DateTime.Now;
                }
            }
        }

        protected void TogglePower()
        {
            if (_dehydrator.Running)
            {
                Debug.Print("PowerButtonClicked, _running == true, turning off.");
                _dehydrator.TurnOff(10);
            }
            else
            {
                Debug.Print("PowerButtonClicked, _running == false, turning on.");
                _dehydrator.TurnOn(_targetTemp, _runTime); // set to 35C to start
            }
        }

        public void Run()
        {
            bool networkInit = Netduino.Foundation.Network.Initializer.InitializeNetwork("http://google.com");

            if (networkInit)
            {
                server.Start();
                Debug.Print("Maple server started.");
            }
        }

        public void Stop()
        {
            server.Stop();
            Debug.Print("Maple server stopped.");
        }

        private float Handler_GetStatus()
        {
            if (_dehydrator.Running)
            {
                return _currentTemp;
            }
            else
            {
                return -1;
            }
        }

        private void Handler_TurnOn(int targetTemp)
        {
            _dehydrator.TurnOn(targetTemp);
        }

        private void Handler_TurnOff(int coolDownDelay)
        {
            _dehydrator.TurnOff(coolDownDelay);
        }

        public static string PadLeft(string text, char filler, int size)
        {
            string padded = string.Empty;
            for (int i = text.Length; i < size; i++)
            {
                padded += filler;
            }
            return padded + text;
        }
    }
}
