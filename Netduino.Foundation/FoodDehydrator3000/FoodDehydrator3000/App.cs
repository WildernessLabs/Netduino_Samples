using FoodDehydrator3000.Properties;
using Maple;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using Netduino.Foundation.Displays;
using Netduino.Foundation.Displays.LCD;
using Netduino.Foundation.Displays.TextDisplayMenu;
using Netduino.Foundation.Generators;
using Netduino.Foundation.Relays;
using Netduino.Foundation.Sensors;
using Netduino.Foundation.Sensors.Rotary;
using Netduino.Foundation.Sensors.Temperature;
using Netduino.Foundation.ICs.IOExpanders.MCP23008;
using System;
using System.Collections;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Network;
using System.Threading;

namespace FoodDehydrator3000
{
    public class App
    {
        // peripherals
        protected PushButton _pushButton = null;
        protected AnalogTemperature _tempSensor = null;
        protected SoftPwm _heaterRelayPwm = null;
        protected Relay _fanRelay = null;
        protected PushButton _button = null;
        protected ITextDisplay _display = null;

        RotaryEncoderWithButton _encoder = null;

        // controllers
        protected DehydratorController _dehydrator = null;

        // vars
        protected float _currentTemp;
        protected MapleServer _server;
        protected Menu _menu;
        protected DateTime _tempUpdated;
        protected float _targetTemp;
        protected TimeSpan _runTime;
        protected bool _inMenu = false;

        public App()
        {
            // initialze
            this.InitializePeripherals();
            this.InitializeMenu();
            //TODO: remove this when we get rid of reinitialize display
            _menu.UpdateItemValue("power", "Turn on");

            // initilaize our dehydrator controller
            _dehydrator = new DehydratorController(_tempSensor, _heaterRelayPwm, _fanRelay, _display);

            // show our info screen
            this.DisplayInfoScreen();

            // 
            this.WireUpPeripheralEvents();

            // setup our web server
            this.InitializeWebServer();
        }

        /// <summary>
        /// Configures the hardware perihperals (LCD, temp sensor, relays, etc.) 
        /// so they can be used by the application.
        /// </summary>
        protected void InitializePeripherals()
        {
            // pushbutton (for testing)
            _pushButton = new PushButton(
                (H.Cpu.Pin)0x15, CircuitTerminationType.Floating);

            // Rotary Encoder
            _encoder = new RotaryEncoderWithButton(
                N.Pins.GPIO_PIN_D7, N.Pins.GPIO_PIN_D6, N.Pins.GPIO_PIN_D5,
                CircuitTerminationType.CommonGround);

            // LCD
            InitializeDisplay();
            Debug.Print("Display up.");
            _display.WriteLine("Display up!", 0);

            // Analog Temp Sensor. Setup to notify at half a degree changes
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A0,
                AnalogTemperature.KnownSensorType.LM35, temperatureChangeNotificationThreshold: 0.5F);
            Debug.Print("TempSensor up.");
            _display.WriteLine("Temp Sensor up!", 1);

            // Heater driven by Software PWM
            _heaterRelayPwm = new SoftPwm(N.Pins.GPIO_PIN_D2, 0.5f, 1.0f / 30.0f);
            Debug.Print("Heater PWM up.");
            _display.WriteLine("Heater PWM up!", 2);

            // Fan Relay
            _fanRelay = new Relay(N.Pins.GPIO_PIN_D3);
            Debug.Print("Fan up.");
            _display.WriteLine("Fan up!", 3);

            // output status
            Debug.Print("Peripherals up");
            _display.WriteLine("Peripherals online!", 0);
        }

        /// <summary>
        /// HACK: 
        /// </summary>
        protected void InitializeDisplay()
        {
            //_display = new Lcd2004(new MCP23008());
            _display = new Lcd2004(N.Pins.GPIO_PIN_D8, N.Pins.GPIO_PIN_D9, N.Pins.GPIO_PIN_D10, N.Pins.GPIO_PIN_D11, N.Pins.GPIO_PIN_D12, N.Pins.GPIO_PIN_D13);
            _display.Clear();
        }

        //
        protected void WireUpPeripheralEvents()
        {
            // Analog Temp Sensor. Setup to notify at half a degree changes
            _tempSensor.TemperatureChanged += (object sender, SensorFloatEventArgs e) => {
                _currentTemp = e.CurrentValue;
                UpdateInfoScreen();
            };

            _pushButton.LongPressClicked += (s, e) => {
                Debug.Print("Long press.");
                _targetTemp = 40;
                TogglePower();
            };

            _encoder.Clicked += (s, e) =>
            {
                // if the menu isn't displayed, display it. otherwise
                // encoder click events are handled by menu
                if (!_inMenu) {
                    this.DisplayMenu();
                }
            };
        }

        protected void InitializeMenu()
        {
            // initialize menu
            _menu = new Menu(_display, _encoder, Resources.GetBytes(Resources.BinaryResources.menu), true);
            _menu.ValueChanged += HandleMenuValueChange;
            _menu.Selected += HandleMenuSelected;
            _menu.Exited += (s, e) => {
                this._inMenu = false;
                this.DisplayInfoScreen();
            };
            // TODO: uncomment when we get rid of ReinitializeDisplay
            //_menu.UpdateItemValue("power", "Turn on");
        }

        /// <summary>
        /// HACK: 
        /// </summary>
        protected void ReinitializeDisplay()
        {
            Thread th = new Thread(() => {
                Thread.Sleep(1500);
                InitializeDisplay();
                InitializeMenu();
                DisplayInfoScreen();
            });
            th.Start();
        }

        protected void InitializeWebServer()
        {
            // configure our web server
            RequestHandler handler = new RequestHandler();
            handler.TurnOff += Handler_TurnOff;
            handler.TurnOn += Handler_TurnOn;
            handler.GetStatus += Handler_GetStatus;

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        /// <summary>
        /// Closes the menu (if open), and displays the info screen which 
        /// has temp and such on it.
        /// </summary>
        protected void DisplayInfoScreen()
        {
            if(_inMenu) CloseMenu();
            UpdateInfoScreen();
        }

        protected void UpdateInfoScreen()
        {
            // if we're in the menu, get out. 
            if (_inMenu) return;

            _display.WriteLine("Current Temp: " + _tempSensor.Temperature.ToString("F1") + "C", 0);
            _display.WriteLine("Target Temp: " + _targetTemp.ToString("F0") + "C", 1);
            var remainingTime = _dehydrator.RunningTimeLeft;
            _display.WriteLine("Time: " + PadLeft(remainingTime.Hours.ToString(), '0', 2) + ":" + PadLeft(remainingTime.Minutes.ToString(), '0', 2), 2);
            _display.WriteLine("Click for more.", 3);
        }

        /// <summary>
        /// Displays the menu.
        /// </summary>
        protected void DisplayMenu()
        {
            this._inMenu = true;
            this._menu.Enable();
        }

        /// <summary>
        /// Closes the menu and displays the info screen.
        /// </summary>
        protected void CloseMenu()
        {
            this._menu.Disable();
            this._inMenu = false;
            this.DisplayInfoScreen();
        }

        /// <summary>
        /// Called when an item in the menu is selected.
        /// </summary>
        protected void HandleMenuSelected(object sender, MenuSelectedEventArgs e)
        {
            switch (e.Command)
            {
                case "power":
                    Debug.Print("menu power");
                    TogglePower();
                    break;
                case "Exit":
                    this.DisplayInfoScreen();
                    break;
            }
        }

        /// <summary>
        /// Called when an item in the menu changes.
        /// </summary>
        protected void HandleMenuValueChange(object sender, ValueChangedEventArgs e)
        {
            switch (e.ItemID) {
                case "temperature":
                    _targetTemp = (float)(double)e.Value; //smh
                    _dehydrator.TargetTemperature = _targetTemp;
                    break;
                case "timer":
                    //TODO: shouldn't this get updated on the dehydrator controller?
                    _runTime = (TimeSpan)e.Value;
                    break;
            }
        }
       

        protected void TogglePower()
        {
            if (_dehydrator.Running) TogglePower(false);
            else TogglePower(true);
        }

        protected void TogglePower(bool turnOn)
        {
            if (turnOn)
            {
                Debug.Print("TogglePower, turning on.");
                _dehydrator.TurnOn(_targetTemp, _runTime); // set to 35C to start
            }
            else
            {
                Debug.Print("TogglePower, turning off.");
                _dehydrator.TurnOff(10);
            }
            // update our menu state
            _menu.UpdateItemValue("power", (_dehydrator.Running) ? "Turn off" : "Turn on");
            UpdateInfoScreen();
        }

        public void Run()
        {
            bool networkInit = Initializer.InitializeNetwork("http://google.com");

            if (networkInit)
            {
                _menu.UpdateItemValue("IP", Initializer.CurrentNetworkInterface.IPAddress.ToString());
                _server.Start();
                Debug.Print("Maple server started.");
            }
        }

        public void Stop()
        {
            _server.Stop();
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
            _targetTemp = targetTemp;
            TogglePower(true);
        }

        private void Handler_TurnOff(int coolDownDelay)
        {
            _targetTemp = 0.0f;
            TogglePower(false);
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
