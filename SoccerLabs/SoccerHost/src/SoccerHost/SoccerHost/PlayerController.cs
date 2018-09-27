using System.Threading;
using Netduino.Foundation.Servos;

namespace SoccerHost
{
    public class PlayerController
    {
        protected bool _isRotating;
        protected bool _rotateRight;
        protected Servo _servo;

        public PlayerController(Servo servo)
        {
            _servo = servo;

            WaitingNetworkConnection();
        }

        public void WaitingNetworkConnection()
        {
            _isRotating = true;

            Thread _animationThread = new Thread(() =>
            {
                int _rotationAngle = 0;

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

        public void NetworkConnected()
        {
            _isRotating = false;
            _servo.RotateTo(0);
        }

        public void Salute()
        {
            Thread _animationThread = new Thread(() =>
            {
                _servo.RotateTo(90);
                Thread.Sleep(1000);
                _servo.RotateTo(0);
            });
            _animationThread.Start();
        }

        public void Kick()
        {
            Thread _animationThread = new Thread(() =>
            {
                _rotateRight = !_rotateRight;

                if (_rotateRight)
                {
                    _servo.RotateTo(180);
                }
                else
                {
                    _servo.RotateTo(0);
                }
            });
            _animationThread.Start();
        }
    }
}
