using Microsoft.SPOT;
using Netduino.Foundation;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Servos;
using System.Threading;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace ChickenCoop.DoorTest
{
    public class App
    {
        protected IContinuousRotationServo _doorServo = null;
        protected PushButton _button = null;
        protected bool _operatingDoor = false;
        protected int _servoTurnsToOperateDoor = 10;
        protected double _totalOpenServoAngle;
        protected DoorOperationDirection _doorState = DoorOperationDirection.Close;
        protected RotationDirection _openDirection = RotationDirection.Clockwise;
        protected RotationDirection _closeDirection = RotationDirection.CounterClockwise;

        public App()
        {
            _doorServo = new ContinuousRotationServo(N.PWMChannels.PWM_PIN_D9, NamedServoConfigs.IdealContinuousRotationServo);
            _button = new PushButton((H.Cpu.Pin)0x15, CircuitTerminationType.Floating);

            _totalOpenServoAngle = _servoTurnsToOperateDoor * 360;


            _button.Clicked += (object sender, EventArgs e) => {
                Debug.Print("Button Clicked");
                ToggleDoor();
            };
        }

        public void Run()
        { }

        protected void ToggleDoor()
        {
            // if we're already in process
            if (_operatingDoor) return;

            if (_doorState == DoorOperationDirection.Close)
            {
                OperateDoor(DoorOperationDirection.Open);
            } else
            {
                OperateDoor(DoorOperationDirection.Close);
            }
        }

        protected void OperateDoor(DoorOperationDirection direction)
        {
            // if we're already in process
            if (_operatingDoor) return;

            _operatingDoor = true;

            Debug.Print("OperateDoor: " + (direction == DoorOperationDirection.Open ? "open" : "close"));

            // open the door
            if (direction == DoorOperationDirection.Open)
            {
                // rotate the winch servo in the open direction
                _doorServo.Rotate(_openDirection, 1.0f);

                // TODO: Rotate until the stop is hit
                Thread.Sleep(3000);

                // stop the winch
                _doorServo.Stop();

                // update our door state
                _doorState = DoorOperationDirection.Open;
                
            } else {
                // rotate the winch servo in the open direction
                _doorServo.Rotate(_closeDirection, 1.0f);

                // TODO: Rotate until the stop is hit
                Thread.Sleep(3000);

                // stop the winch
                _doorServo.Stop();

                // update our door state
                _doorState = DoorOperationDirection.Close;
            }

            //
            _operatingDoor = false;
        }
    }

    public enum DoorOperationDirection
    {
        Open,
        Close
    }
}
