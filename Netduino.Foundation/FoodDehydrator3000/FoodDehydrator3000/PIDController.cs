using System;
using System.Threading;

namespace FoodDehydrator3000
{
    public class PidController
    {
        protected DateTime _lastUpdateTime;
        protected float _lastError;
        protected float _integral;

        public float Input { get; set; }
        public float TargetInput { get; set; }

        public float P { get; set; } = 1;
        public float I { get; set; } = 0;
        public float D { get; set; } = 0;

        public PidController()
        {
            _lastUpdateTime = DateTime.Now;
            _lastError = 0;
            _integral = 0;
        }

        public void ResetIntegrator()
        {
            _integral = 0;
        }

        public float CalculatePowerOutput()
        {
            var now = DateTime.Now;
            // time delta (how long since last calculation)
            var dt = now - _lastUpdateTime;

            // if no time has passed, don't make any changes.
            if (dt.Ticks <= 0.0) return Input;

            // copy vars
            var input = Input;
            var target = TargetInput;

            // calculate the error (how far we are from target)
            var error = target - input;

            // calculate the integral
            _integral += error * (float)dt.Ticks;
            var diff = error / (float)dt.Ticks;

            // PID!
            var control = P * error + I * _integral + D * diff;

            _lastUpdateTime = now;

            //ControlOutput = control;
            return control;
        }
    }
}