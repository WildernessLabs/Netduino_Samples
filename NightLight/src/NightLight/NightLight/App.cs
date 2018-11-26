using Netduino.Foundation.LEDs;
using Netduino.Foundation.Sensors.Motion;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace NightLight
{
    public class App
    {
        protected RgbPwmLed rgbLed;
        protected ParallaxPIR parallaxPIR;

        public App()
        {
            InitializePeripherals();
        }

        private void InitializePeripherals()
        {
            rgbLed = new RgbPwmLed
            (
                N.PWMChannels.PWM_PIN_D11,
                N.PWMChannels.PWM_PIN_D10,
                N.PWMChannels.PWM_PIN_D9,
                2.1f,
                3.0f,
                3.0f,
                false
            );
            rgbLed.SetColor(Netduino.Foundation.Color.Green);

            parallaxPIR = new ParallaxPIR(N.Pins.GPIO_PIN_D8) ;
            parallaxPIR.OnMotionStart += ParallaxPIRMotionStart;
            parallaxPIR.OnMotionEnd += ParallaxPIRMotionEnd;
        }

        private void ParallaxPIRMotionStart(object sender)
        {
            rgbLed.StartPulse(Netduino.Foundation.Color.Red);
        }

        private void ParallaxPIRMotionEnd(object sender)
        {
            rgbLed.SetColor(Netduino.Foundation.Color.Green);
        }

        public void Run()
        {

        }
    }
}
