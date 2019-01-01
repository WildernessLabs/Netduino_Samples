using Netduino.Foundation.Motors;

namespace CarHost
{
    public class CarController
    {
        protected HBridgeMotor _motorLeft;
        protected HBridgeMotor _motorRight;

        public CarController(HBridgeMotor motorLeft, HBridgeMotor motorRight)
        {
            _motorLeft = motorLeft;
            _motorRight = motorRight;
        }

        public void Stop()
        {
            _motorLeft.Speed = 0f;
            _motorRight.Speed = 0f;
        }

        public void TurnLeft()
        {
            _motorLeft.Speed = 1f;
            _motorRight.Speed = -1f;
        }

        public void TurnRight()
        {
            _motorLeft.Speed = -1f;
            _motorRight.Speed = 1f;
        }

        public void MoveForward()
        {
            _motorLeft.Speed = -1f;
            _motorRight.Speed = -1f;
        }

        public void MoveBackward()
        {
            _motorLeft.Speed = 1f;
            _motorRight.Speed = 1f;
        }
    }
}
