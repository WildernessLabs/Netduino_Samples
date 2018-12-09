using Netduino.Foundation.Motors;

namespace CarHost
{
    public class CarController
    {
        protected HBridgeMotor _motor1;
        protected HBridgeMotor _motor2;

        public CarController(HBridgeMotor motor1, HBridgeMotor motor2)
        {
            _motor1 = motor1;
            _motor2 = motor2;

            //TurnRight();
        }

        public void Stop()
        {
            _motor1.Speed = 0f;
            _motor2.Speed = 0f;
        }

        public void TurnLeft()
        {
            _motor1.Speed = 1f;
            _motor2.Speed = -1f;
        }

        public void TurnRight()
        {
            _motor1.Speed = -1f;
            _motor2.Speed = 1f;
        }

        public void MoveForward()
        {
            _motor1.Speed = -1f;
            _motor2.Speed = -1f;
        }

        public void MoveBackward()
        {
            _motor1.Speed = 1f;
            _motor2.Speed = 1f;
        }
    }
}
