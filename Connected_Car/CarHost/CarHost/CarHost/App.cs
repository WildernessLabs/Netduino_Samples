using Microsoft.SPOT;
using System.Threading;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors.Distance;
using Netduino.Foundation.Motors;

namespace CarHost
{
    public class App
    {
        protected HBridgeMotor motor1;
        protected HBridgeMotor motor2;
        protected HCSR04 distanceSensor;

        public App()
        {
            InitializePeripherals();
        }

        protected void InitializePeripherals()
        {
            motor1 = new HBridgeMotor(N.PWMChannels.PWM_PIN_D3, N.PWMChannels.PWM_PIN_D5, N.Pins.GPIO_PIN_D4);
            motor2 = new HBridgeMotor(N.PWMChannels.PWM_PIN_D6, N.PWMChannels.PWM_PIN_D10, N.Pins.GPIO_PIN_D7);
            distanceSensor = new HCSR04(N.Pins.GPIO_PIN_D12, N.Pins.GPIO_PIN_D11);
            distanceSensor.DistanceDetected += OnDistanceDetected;
        }

        protected void Forward()
        {
            motor1.Speed = -1f;
            motor2.Speed = -1f;
        }

        protected void Stop()
        {
            motor1.Speed = 0f;
            motor2.Speed = 0f;
        }

        protected void OnDistanceDetected(object sender, DistanceEventArgs e)
        {
            if (e.Distance == -1 || e.Distance > 30)
                Forward();
            else
                Stop();

            Debug.Print("current distance: " + e.Distance);
        }

        public void Run()
        {
            while (true)
            {
                //// set the speed on both motors to 100% forward
                //motor1.Speed = 0.5f;
                //motor2.Speed = 0.5f;
                //Thread.Sleep(1000);

                //motor1.Speed = 0f;
                //motor2.Speed = 0f;
                //Thread.Sleep(500);

                //// 100% reverse
                //motor1.Speed = -0.5f;
                //motor2.Speed = -0.5f;
                //Thread.Sleep(1000);

                distanceSensor.MeasureDistance();
                Thread.Sleep(250);
            }
        }
    }
}