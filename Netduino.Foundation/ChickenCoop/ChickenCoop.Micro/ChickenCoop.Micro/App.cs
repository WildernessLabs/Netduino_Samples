using System;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using Netduino.Foundation;
using Netduino.Foundation.Generators;
using Netduino.Foundation.Sensors.Temperature;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Displays.LCD;
using Netduino.Foundation.Displays;
using Netduino.Foundation.Sensors.Rotary;
using ChickenCoop.Micro.Door;
using Netduino.Foundation.Servos;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Displays.TextDisplayMenu;
using ChickenCoop.Micro.Properties;

namespace ChickenCoop.Micro
{
    public class App
    {
        public static App Current = null;

        // stuff
        protected Menu _menu;
        protected bool _inMenu = false;

        // peripherals
        protected IContinuousRotationServo _doorServo = null;
        protected PushButton _openEndStopSwitch = null;
        protected PushButton _closeEndStopSwitch = null;
        protected SoftPwm _heatLampRelay;
        protected AnalogTemperature _tempSensor;
        protected ITextDisplay _display = null;
        protected RotaryEncoderWithButton _encoder = null;

        // controllers
        protected DoorController _doorController;
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

            // initialize our controllers
            InitializeControllers();
        }

        public void Run()
        {
            //_tempController.TargetTemperature = 15.0f; //15C/60F
            //_tempController.Run();
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
            // door stuff
            _doorServo = new ContinuousRotationServo(N.PWMChannels.PWM_PIN_D9, NamedServoConfigs.IdealContinuousRotationServo);
            _openEndStopSwitch = new PushButton(N.Pins.GPIO_PIN_D2, CircuitTerminationType.CommonGround);
            _closeEndStopSwitch = new PushButton(N.Pins.GPIO_PIN_D3, CircuitTerminationType.CommonGround);

            // temp stuff
            _heatLampRelay = new SoftPwm(N.Pins.GPIO_PIN_D2, 0, 1 / 60);
            _tempSensor = new AnalogTemperature(N.AnalogChannels.ANALOG_PIN_A2, AnalogTemperature.KnownSensorType.LM35, updateInterval: 5000, temperatureChangeNotificationThreshold: 1.0f);

        }

        protected void InitializeControllers()
        {
            _doorController = new DoorController(_doorServo, _openEndStopSwitch, _closeEndStopSwitch);
            _doorController.DoorOpened += (s, e) => {
                _menu.UpdateItemValue("toggle", "Close");
            };
            _doorController.DoorOpened += (s, e) => {
                _menu.UpdateItemValue("toggle", "Open");
            };
            _tempController = new TemperatureController(_heatLampRelay, _tempSensor);
        }

        protected void InitializeMenu()
        {
            // initialize menu
            _menu = new Menu(_display, _encoder, Resources.GetBytes(Resources.BinaryResources.menu), true);
            //_menu.ValueChanged += HandleMenuValueChange;
            _menu.Selected += HandleMenuSelected;
            _menu.Exited += (s, e) => {
                this._inMenu = false;
                this.DisplayInfoScreen();
            };
        }

        /// <summary>
        /// Closes the menu (if open), and displays the info screen which 
        /// has temp and such on it.
        /// </summary>
        protected void DisplayInfoScreen()
        {
            if (_inMenu) CloseMenu();
            UpdateInfoScreen();
        }

        protected void UpdateInfoScreen()
        {
            // if we're in the menu, get out. 
            if (_inMenu) return;

            //_display.WriteLine("Current Temp: " + _tempSensor.Temperature.ToString("F1") + "C", 0);
            //_display.WriteLine("Target Temp: " + _targetTemp.ToString("F0") + "C", 1);
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
                case "toggleDoor":
                    Debug.Print("Menu: ToggleDoor");
                    ToggleDoor();
                    break;
                case "Exit":
                    this.DisplayInfoScreen();
                    break;
            }
        }

        ///// <summary>
        ///// Called when an item in the menu changes.
        ///// </summary>
        //protected void HandleMenuValueChange(object sender, ValueChangedEventArgs e)
        //{
        //    switch (e.ItemID)
        //    {
        //        case "temperature":
        //            _targetTemp = (float)(double)e.Value; //smh
        //            _dehydrator.TargetTemperature = _targetTemp;
        //            break;
        //        case "timer":
        //            //TODO: shouldn't this get updated on the dehydrator controller?
        //            _runTime = (TimeSpan)e.Value;
        //            break;
        //    }
        //}

        protected void ToggleDoor()
        {
            _doorController.ToggleDoor();
        }

    }
}
