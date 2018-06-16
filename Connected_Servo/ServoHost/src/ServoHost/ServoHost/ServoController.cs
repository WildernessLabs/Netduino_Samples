using Netduino.Foundation.Servos;
using System.Threading;

namespace ServoHost
{
    public class ServoController
    {
        protected bool _isRotating;
        protected Servo _servo;
        protected Thread _animationThread = null;

        public ServoController(Servo servo)
        {
            _servo = servo;
            StartCycling();
        }

        public void GoToAngleZero()
        {
            StopCycling();
            _servo.RotateTo(0);
        }

        public void StartCycling()
        {
            _isRotating = true;
            _animationThread = new Thread(() =>
            {
                while (_isRotating)
                {
                    for (int i = 0; i < 270; i++)
                    {
                        if (!_isRotating)
                            break;

                        _servo.RotateTo(i);
                        Thread.Sleep(25);
                    }
                }
            });
            _animationThread.Start();
        }

        public void StopCycling()
        {
            _isRotating = false;
        }

        public void NetworkConnected()
        {
            StopCycling();
            _servo.RotateTo(0);
        }
    }
}