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
    public class DehydratorController
    {
        // events
        public event EventHandler RunTimeElapsed = delegate { };

        // peripherals
        protected AnalogTemperature _tempSensor = null;
        protected SoftPwm _heaterRelayPwm = null;
        protected Relay _fanRelay = null;
        protected SerialLCD _display = null;

        // controllers
        PidController _pidController = null;

        // other members
        Thread _tempControlThread = null;

        // properties
        public bool Running {
            get { return _running; }
        }
        protected bool _running = false;

        public float TargetTemperature { get; set; }

        public TimeSpan RunningTimeLeft
        {
            get { return _runningTimeLeft; }
        }
        protected TimeSpan _runningTimeLeft = TimeSpan.MinValue;

        public DehydratorController(AnalogTemperature tempSensor, SoftPwm heater, Relay fan, SerialLCD display)
        {
            _tempSensor = tempSensor;
            _heaterRelayPwm = heater;
            _fanRelay = fan;
            _display = display;

            _pidController = new PidController(45000);
            _pidController.P = 0.001;
            _pidController.I = 0.00001;
            _pidController.D = 0;

        }


        public void TurnOff()
        {
            Debug.Print("Turning off.");
            this._fanRelay.IsOn = false;
            this._heaterRelayPwm.Stop();
            this._running = false;
            this._runningTimeLeft = TimeSpan.MinValue;
        }

        public void TurnOn()
        {
            TurnOn(TimeSpan.MaxValue);
        }

        public void TurnOn(TimeSpan runningTime)
        {
            // set our state vars
            Debug.Print("Turning on.");
            this._runningTimeLeft = runningTime;
            this._running = true;

            // start our temp regulation thread. might want to change this to notify.
            StartRegulatingTemperatureThread();

            Debug.Print("Here");

            // TEMP - to be replaced with PID stuff
            this._fanRelay.IsOn = true;
            this._heaterRelayPwm.Frequency = 1.0f / 10.0f; // 10 seconds
            this._heaterRelayPwm.DutyCycle = 0.5f; // 50% on
            this._heaterRelayPwm.Start();
        }

        public void PowerButtonClicked()
        {
            if (this._running) {
                Debug.Print("PowerButtonClicked, _running == true, turning off.");
                this.TurnOff();
            } else {
                Debug.Print("PowerButtonClicked, _running == false, turning on.");
                this.TurnOn();
            }
        }

        protected void StartRegulatingTemperatureThread()
        {
            _tempControlThread = new Thread(() => {
                while (this._running) {
                    Debug.Print("Temp: " + _tempSensor.Temperature.ToString() + "ºC");
                    _display.WriteLine("Temp: " + _tempSensor.Temperature.ToString(), 1);
                    Thread.Sleep(2000);
                    _pidController.Input = _tempSensor.Temperature;
                    _pidController.TargetInput = this.TargetTemperature;

                    //var powerLevel = _pidController.CalculatePower();
                    //this._heaterRelayPwm.DutyCycle = powerLevel();
                }
            });
            _tempControlThread.Start();
        }
    }
}
