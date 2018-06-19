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
            StartCycling();
        }

        public void RotateTo(int degrees)
        {
            StopCycling();
            _servo.RotateTo(degrees);
        }

        public void StopCycling()
        {
            _isRotating = false;
        }

        public void StartCycling()
        {
            StopCycling();

            _isRotating = true;
            _animationThread = new Thread(() =>
            {
                while (_isRotating)
                {
                    if (_rotationAngle == 270)
                        _rotationAngle = 0;
                    else
                        _rotationAngle++;

                    _servo.RotateTo(_rotationAngle);
                    Thread.Sleep(25);
                }
            });
            _animationThread.Start();
        }

        public void NetworkConnected()
        {
            StopCycling();
            _servo.RotateTo(0);
        }
    }
}