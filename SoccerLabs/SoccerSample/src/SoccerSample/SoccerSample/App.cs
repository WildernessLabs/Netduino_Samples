using System;
using Netduino.Foundation.Servos;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Hardware;

namespace SoccerSample
{
    public class App
    {
        protected InterruptPort _buttonTeamA;
        protected PlayerController _playerTeamA;

        protected InterruptPort _buttonTeamB;
        protected PlayerController _playerTeamB;

        public App()
        {
            InitializePeripherals();
        }

        protected void InitializePeripherals()
        {
            _buttonTeamA = new InterruptPort(N.Pins.GPIO_PIN_D4, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            _buttonTeamA.OnInterrupt += OnButtonTeamA;

            var servoA = new Servo(N.PWMChannels.PWM_PIN_D3, NamedServoConfigs.Ideal180Servo);
            _playerTeamA = new PlayerController(servoA);

            _buttonTeamB = new InterruptPort(N.Pins.GPIO_PIN_D10, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            _buttonTeamB.OnInterrupt += OnButtonTeamB;

            var servoB = new Servo(N.PWMChannels.PWM_PIN_D11, NamedServoConfigs.Ideal180Servo);
            _playerTeamB = new PlayerController(servoB);
        }

        private void OnButtonTeamA(uint data1, uint data2, DateTime time)
        {
            _playerTeamA.Kick();
        }

        private void OnButtonTeamB(uint data1, uint data2, DateTime time)
        {
            _playerTeamB.Kick();
        }

        public void Run()
        {

        }
    }
}
