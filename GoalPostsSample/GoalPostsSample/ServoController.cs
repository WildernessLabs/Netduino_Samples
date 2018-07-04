using System.Threading;
using Netduino.Foundation.Servos;

namespace GoalPostsSample
{
    public class ServoController
    {
        protected bool _rotateRight;
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
