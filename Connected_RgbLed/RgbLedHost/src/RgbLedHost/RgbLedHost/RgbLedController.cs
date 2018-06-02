using Netduino.Foundation.LEDs;
using System;
using System.Collections;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace RgbLedHost
{
    public class RgbLedController
    {
        protected RgbPwmLed rgbPwmLed;

        public RgbLedController()
        {
            rgbPwmLed = new RgbPwmLed
            (
                N.PWMChannels.PWM_PIN_D11,
                N.PWMChannels.PWM_PIN_D10,
                N.PWMChannels.PWM_PIN_D9,
                1.05f,
                1.5f,
                1.5f,
                false
            );

            rgbPwmLed.SetColor(Netduino.Foundation.Color.Red);
        }

        public void LightOn()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.SetColor(GetRandomColor());
        }

        public void LightOff()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.SetColor(Netduino.Foundation.Color.FromHsba(0, 0, 0));
        }

        public void Blink()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.StartBlink(GetRandomColor());
        }

        public void Pulse()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.StartPulse(GetRandomColor());
        }

        public void RunningColors()
        {
            var arrayColors = new ArrayList();
            for (int i = 0; i < 360; i = i + 5)
            {
                var hue = ((double)i / 360F);
                arrayColors.Add(Netduino.Foundation.Color.FromHsba(((double)i / 360F), 1, 1));
            }

            int[] intervals = new int[arrayColors.Count];
            for (int i = 0; i < intervals.Length; i++)
            {
                intervals[i] = 100;
            }

            rgbPwmLed.Stop();
            rgbPwmLed.StartRunningColors(arrayColors, intervals);
        }

        public void NetworkConnected()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
        }

        protected Netduino.Foundation.Color GetRandomColor()
        {
            var random = new Random();

            return Netduino.Foundation.Color.FromHsba(random.NextDouble(), 1, 1);
        }
    }
}
