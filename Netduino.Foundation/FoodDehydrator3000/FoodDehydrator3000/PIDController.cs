using System;
using System.Threading;

namespace FoodDehydrator3000
{
    public class PidController
    {
        protected DateTime _lastUpdateTime;
        protected double _lastError;
        protected double _integral;
        private readonly ushort _updateInterval = 100;
        //public const ushort MinimumPollingPeriod = 100;

        public double Input { get; set; }
        public double TargetInput { get; set; }

        public double P { get; set; } = 1;
        public double I { get; set; } = 0;
        public double D { get; set; } = 0;

        public double ControlOutput { get; private set; }

        //public ushort UpdateInteval { get; set; }


        public PidController(ushort updateInterval = 100)
        {
            _updateInterval = updateInterval;

            //Input = AddInput("Input", Units.Scalar);
            //TargetInput = AddInput("TargetInput", Units.Scalar);

            //ControlOutput = AddOutput("ControlOutput", Units.Scalar, 0);

            _lastUpdateTime = DateTime.Now;
            _lastError = 0;
            _integral = 0;

            this.StartUpdating();
        }

        public void ResetIntegrator()
        {
            _integral = 0;
        }

        /// <summary>
        ///     Start the update process.
        /// </summary>
        protected void StartUpdating()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    Update();
                    Thread.Sleep(_updateInterval);
                }
            });
            t.Start();
        }

        protected void Update()
        {
            var now = DateTime.Now;
            var dt = now - _lastUpdateTime;

            if (dt.Ticks <= 0.0) return;

            var input = Input;
            var target = TargetInput;

            var error = target - input;

            _integral += error * (double)dt.Ticks;
            var diff = error / (double)dt.Ticks;

            var control = P * error + I * _integral + D * diff;

            _lastUpdateTime = now;

            ControlOutput = control;
        }
    }
}