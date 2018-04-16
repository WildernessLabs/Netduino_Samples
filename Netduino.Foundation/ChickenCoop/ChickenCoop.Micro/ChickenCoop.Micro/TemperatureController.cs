using System;
using Microsoft.SPOT;
using Netduino.Foundation.Sensors.Temperature;
using Netduino.Foundation.Generators;
using System.Threading;
using Netduino.Foundation.Controllers.PID;

namespace ChickenCoop.Micro
{
    public class TemperatureController
    {
        SoftPwm _heatLampRelay;
        AnalogTemperature _tempSensor;
        Thread _tempControlThread = null;
        int _powerUpdateInterval = 60000; // milliseconds; how often to update the power
        public float TargetTemperature { get; set; }
        // controllers
        IPidController _pidController = null;

        // properties
        public bool Running
        {
            get { return _running; }
        }
        protected bool _running = false;

        public TemperatureController(SoftPwm heatLampRelay, AnalogTemperature tempSensor)
        {
            // store references to the peripherals
            _heatLampRelay = heatLampRelay;
            _tempSensor = tempSensor;

            // configure our PID controller
            _pidController = new StandardPidController();
            _pidController.ProportionalComponent = .5f; // proportional
            _pidController.IntegralComponent = .55f; // integral time minutes
            _pidController.DerivativeComponent = 0f; // derivative time in minutes
            _pidController.OutputMin = 0.0f; // 0% power minimum
            _pidController.OutputMax = 1.0f; // 100% power max
            _pidController.OutputTuningInformation = true;

        }

        public void Run()
        {
            // only allow if not running
            if (_running) return;

            // start regulating temp
            _running = true;
            this.StartRegulatingTemperatureThread();

        }

        protected void StartRegulatingTemperatureThread()
        {
            _tempControlThread = new Thread(() => {

                // reset our integral history
                _pidController.ResetIntegrator();

                while (_running)
                {

                    // set our input and target on the PID calculator
                    _pidController.ActualInput = _tempSensor.Temperature;
                    _pidController.TargetInput = this.TargetTemperature;

                    // get the appropriate power level (only use PI, since the temp signal is noisy)
                    var powerLevel = _pidController.CalculateControlOutput();
                    //Debug.Print("Temp: " + _tempSensor.Temperature.ToString() + "/" + TargetTemperature.ToString("N0") + "ºC");

                    // set our PWM appropriately
                    //Debug.Print("Setting duty cycle to: " + (powerLevel * 100).ToString("N0") + "%");
                    //_display.WriteLine("Power: " + (powerLevel * 100).ToString("N0") + "%", 0);

                    this._heatLampRelay.DutyCycle = powerLevel;

                    // sleep for a while. 
                    Thread.Sleep(_powerUpdateInterval);
                }
            });
            _tempControlThread.Start();
        }
    }
}
