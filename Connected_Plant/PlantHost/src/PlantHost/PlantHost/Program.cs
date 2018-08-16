using System;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.Netduino;
using SLH = SecretLabs.NETMF.Hardware;
using System.Threading;
using Netduino.Foundation.GPIO;

namespace PlantHost
{
    public class Program
    {
        public static void Main()
        {
            var a0 = new SLH.AnalogInput(Pins.GPIO_PIN_A0);
            var d7 = new Microsoft.SPOT.Hardware.OutputPort(Pins.GPIO_PIN_D7, false);

            while (true)
            {
                int sample = 0;
                for (int i = 0; i < 5; i++)
                {
                    d7.Write(true);
                    Thread.Sleep(5);
                    sample += a0.Read();
                    d7.Write(false);
                }

                float result = 100 - Map((sample / 5), 520, 930, 0, 100);
                Debug.Print(result.ToString());

                Thread.Sleep(1000);
            }
        }

        static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            return (((toHigh - toLow) * (value - fromLow)) / (fromHigh - fromLow)) - toLow;
        }
    }
}
