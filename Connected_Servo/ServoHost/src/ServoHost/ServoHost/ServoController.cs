using Netduino.Foundation.Servos;
using System.Threading;

namespace ServoHost
{
    public class ServoController
    {
        protected int _rotationAngle;
        protected bool _isRotating;
        protected Servo _servo;

        public ServoController(Servo servo)
        {
            _servo = servo;
            StartSweep();
        }

        public void RotateTo(int degrees)
        {
            StopSweep();
            _servo.RotateTo(degrees);
        }

        public void StopSweep()
        {
            _isRotating = false;
        }

        public void StartSweep()
        {
            StopSweep();

            _isRotating = true;
            Thread _animationThread = new Thread(() =>
            {
                while (_isRotating)
                {
                    while (_rotationAngle < 180)
                    {
                        if (!_isRotating)
                            break;

                        _rotationAngle++;
                        _servo.RotateTo(_rotationAngle);
                        Thread.Sleep(15);
                    }

                    while (_rotationAngle > 0)
                    {
                        if (!_isRotating)
                            break;

                        _rotationAngle--;
                        _servo.RotateTo(_rotationAngle);
                        Thread.Sleep(15);
                    }
                }
            });
            _animationThread.Start();
        }
    }
}