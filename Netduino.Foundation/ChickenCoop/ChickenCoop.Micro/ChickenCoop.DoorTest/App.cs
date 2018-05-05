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
        protected double _totalOpenServoAngle;
        protected DoorState _doorState = DoorState.Unknown;
        protected RotationDirection _openDirection = RotationDirection.Clockwise;
        protected RotationDirection _closeDirection = RotationDirection.CounterClockwise;
        protected PushButton _openEndStopSwitch = null;
        protected PushButton _closeEndStopSwitch = null;
        protected bool _openEndStopTriggered = false;
        protected bool _closeEndStopTriggered = false;

        public App()
        {
            // instantiate all of our peripherals
            _doorServo = new ContinuousRotationServo(N.PWMChannels.PWM_PIN_D9, NamedServoConfigs.IdealContinuousRotationServo);
            _button = new PushButton((H.Cpu.Pin)0x15, CircuitTerminationType.Floating);
            _openEndStopSwitch = new PushButton(N.Pins.GPIO_PIN_D2, CircuitTerminationType.CommonGround);
            _closeEndStopSwitch = new PushButton(N.Pins.GPIO_PIN_D3, CircuitTerminationType.CommonGround);

            // set our end stop trigger events
            _openEndStopSwitch.PressStarted += (s, e) => {
                _openEndStopTriggered = true; _doorState = DoorState.Open;
                Debug.Print("open end stop triggered");
            };
            _openEndStopSwitch.PressEnded += (s, e) => { _openEndStopTriggered = false; };
            _closeEndStopSwitch.PressStarted += (s, e) => {
                _closeEndStopTriggered = true; _doorState = DoorState.Closed;
                Debug.Print("close end stop triggered");
            };
            _closeEndStopSwitch.PressEnded += (s, e) => { _closeEndStopTriggered = false; };

            // wire up our button click for door open/close
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

            if (_doorState == DoorState.Closed || _doorState == DoorState.Unknown)
            {
                OperateDoor(true);
            }
            else
            {
                OperateDoor(false);
            }
        }

        protected void OperateDoor(bool open)
        {
            // if we're already in process
            // TODO: maybe we make this cancellable
            if (_operatingDoor) return;
            _operatingDoor = true;

            // spin up a new thread so we don't block anything.
            // this is especially important with the end stops. 
            // without a new thread, we don't get those events. :D
            Thread th = new Thread(() =>
            {
                Debug.Print("OperateDoor: " + (open ? "open" : "close"));

                // open the door
                if (open)
                {
                    // rotate the winch servo until the end stop is triggered
                    while (!_openEndStopTriggered)
                    {
                        // rotate the winch servo in the open direction
                        _doorServo.Rotate(_openDirection, 1.0f);
                    }
                    // stop the winch
                    _doorServo.Stop();
                    Debug.Print("Open end stop hit.");

                    // update our door state
                    _doorState = DoorState.Open;

                }
                else
                { // close the door
                  // rotate the winch servo until the end stop is triggered
                    while (!_closeEndStopTriggered)
                    {
                        // close
                        _doorServo.Rotate(_closeDirection, 1.0f);
                    }
                    // stop the winch
                    _doorServo.Stop();
                    Debug.Print("Open end stop hit.");

                    // update our door state
                    _doorState = DoorState.Closed;
                }

                //
                _operatingDoor = false;
            });
            th.Start();
        }
    }

    public enum DoorState
    {
        Open,
        Closed,
        Unknown
    }
}
