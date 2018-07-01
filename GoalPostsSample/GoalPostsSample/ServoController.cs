using System.Threading;
using Netduino.Foundation.Servos;
using System;
using Microsoft.SPOT;

namespace GoalPostsSample
{
    public class ServoController
    {
        protected int _rotationAngle;
        protected bool _isRotating;
        protected Servo _servo;
        protected Thread _animationThread = null;

        public ServoController(Servo servo)
        {
            _servo = servo;
            _servo.RotateTo(0);
        }

        public void StartGoalAnimation()
        {
            _animationThread = new Thread(() =>
            {
                if (_isRotating)
                    return;
                _isRotating = true;

                _rotationAngle = 0;

                while (_rotationAngle < 180)
                {
                    _servo.RotateTo(_rotationAngle);
                    Thread.Sleep(10);
                    _rotationAngle++;
                }

                while (_rotationAngle > 0)
                {
                    _servo.RotateTo(_rotationAngle);
                    Thread.Sleep(10);
                    _rotationAngle--;
                }

                _isRotating = false;
            });
            _animationThread.Start();
        }
    }
}
