using Microsoft.SPOT;
using Netduino.Foundation;
using Netduino.Foundation.Sensors.Buttons;
using Netduino.Foundation.Servos;
using System.Threading;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace ChickenCoop.Micro.Door
{
    public class DoorController
    {
        // events
        public event EventHandler DoorOpened = delegate { };
        public event EventHandler DoorClosed = delegate { };

        // peripherals
        protected IContinuousRotationServo _doorServo = null;
        protected RotationDirection _openDirection = RotationDirection.CounterClockwise;
        protected RotationDirection _closeDirection = RotationDirection.Clockwise;
        protected PushButton _openEndStopSwitch = null;
        protected PushButton _closeEndStopSwitch = null;

        // state
        protected bool _operatingDoor = false;
        protected bool _openEndStopTriggered = false;
        protected bool _closeEndStopTriggered = false;

        public DoorStateType State
        {
            get { return _doorState; }
        }
        protected DoorStateType _doorState = DoorStateType.Unknown;

        /// <summary>
        /// The physical state of the door, based on the end stop switches
        /// </summary>
        public DoorStateType PhysicalState
        {
            get
            {
                if (_openEndStopSwitch.State == _closeEndStopSwitch.State)
                {
                    return DoorStateType.Unknown;
                } else if (_openEndStopSwitch.State) {
                    return DoorStateType.Open;
                } else
                {
                    return DoorStateType.Closed;
                }

            }
        }

        public DoorController(IContinuousRotationServo doorServo, PushButton openEndStopSwitch, PushButton closeEndStopSwitch)
        {
            // instantiate all of our peripherals
            _doorServo = doorServo;
            _openEndStopSwitch = openEndStopSwitch;
            _closeEndStopSwitch = closeEndStopSwitch;

            // set our end stop trigger events
            _openEndStopSwitch.PressStarted += (s, e) => {
                _openEndStopTriggered = true; _doorState = DoorStateType.Open;
                Debug.Print("open end stop triggered");
            };
            _openEndStopSwitch.PressEnded += (s, e) => { _openEndStopTriggered = false; };
            _closeEndStopSwitch.PressStarted += (s, e) => {
                _closeEndStopTriggered = true; _doorState = DoorStateType.Closed;
                Debug.Print("close end stop triggered");
            };
            _closeEndStopSwitch.PressEnded += (s, e) => { _closeEndStopTriggered = false; };

            // load our initial state from the physical state
            this._doorState = PhysicalState;

        }

        public void ToggleDoor()
        {
            // if we're already in process
            if (_operatingDoor) return;

            // update from physical so we don't break things
            _doorState = PhysicalState;

            Debug.Print("DoorController.ToggleDoor, current state: " + _doorState.ToString());

            if (_doorState == DoorStateType.Closed || _doorState == DoorStateType.Unknown)
            {
                OperateDoor(true);
            }
            else
            {
                OperateDoor(false);
            }
        }

        public void OperateDoor(bool open)
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
                        _doorServo.Rotate(_openDirection, 0.9f);
                    }
                    // stop the winch
                    _doorServo.Stop();
                    Debug.Print("Open end stop hit.");

                    // update our door state
                    _doorState = DoorStateType.Open;

                    // raise our event
                    this.DoorOpened(this, new EventArgs());

                }
                else
                { // close the door
                  // rotate the winch servo until the end stop is triggered
                    while (!_closeEndStopTriggered)
                    {
                        // close
                        _doorServo.Rotate(_closeDirection, 0.75f);
                    }
                    // stop the winch
                    _doorServo.Stop();
                    Debug.Print("Open end stop hit.");

                    // update our door state
                    _doorState = DoorStateType.Closed;

                    // raise our event
                    this.DoorClosed(this, new EventArgs());
                }

                //
                _operatingDoor = false;
            });
            th.Start();
        }
    }

}
