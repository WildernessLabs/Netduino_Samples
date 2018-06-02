using Netduino.Foundation.LEDs;
using System;
using System.Collections;

namespace RgbLedHost
{
    public class RgbLedController
    {
        protected RgbPwmLed _rgbPwmLed;

        public RgbLedController(RgbPwmLed rgbPwmLed)
        {
            _rgbPwmLed = rgbPwmLed;
            _rgbPwmLed.SetColor(Netduino.Foundation.Color.Red);
        }

        public void TurnOn()
        {
            _rgbPwmLed.Stop();
            _rgbPwmLed.SetColor(GetRandomColor());
        }

        public void TurnOff()
        {
            _rgbPwmLed.Stop();
            _rgbPwmLed.SetColor(Netduino.Foundation.Color.FromHsba(0, 0, 0));
        }

        public void Blink()
        {
            _rgbPwmLed.Stop();
            _rgbPwmLed.StartBlink(GetRandomColor());
        }

        public void Pulse()
        {
            _rgbPwmLed.Stop();
            _rgbPwmLed.StartPulse(GetRandomColor());
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

            _rgbPwmLed.Stop();
            _rgbPwmLed.StartRunningColors(arrayColors, intervals);
        }

        public void NetworkConnected()
        {
            _rgbPwmLed.Stop();
            _rgbPwmLed.SetColor(Netduino.Foundation.Color.Green);
        }

        protected Netduino.Foundation.Color GetRandomColor()
        {
            var random = new Random();
            return Netduino.Foundation.Color.FromHsba(random.NextDouble(), 1, 1);
        }
    }
}
