using System.Threading;
using Netduino.Foundation.Servos;

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
            _rotationAngle = 0;

            _animationThread = new Thread(() =>
            {
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
            });
            _animationThread.Start();
        }
    }
}
