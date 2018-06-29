using Netduino.Foundation.Servos;
using System.Threading;

namespace ServoHost
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
            StartSweeping();
        }

        public void RotateTo(int degrees)
        {
            StopSweeping();
            _servo.RotateTo(degrees);
        }

        public void StopSweeping()
        {
            _isRotating = false;
        }

        public void StartSweeping()
        {
            StopSweeping();

            _isRotating = true;
            _animationThread = new Thread(() =>
            {
                while (_isRotating)
                {
                    while (_rotationAngle < 180)
                    {
                        if (!_isRotating)
                            break;

                        _servo.RotateTo(_rotationAngle);
                        Thread.Sleep(25);
                        _rotationAngle++;
                    }

                    while (_rotationAngle > 0)
                    {
                        if (!_isRotating)
                            break;

                        _servo.RotateTo(_rotationAngle);
                        Thread.Sleep(25);
                        _rotationAngle--;
                    }
                }
            });
            _animationThread.Start();
        }

        public void NetworkConnected()
        {
            StopSweeping();
            _servo.RotateTo(0);
        }
    }
}