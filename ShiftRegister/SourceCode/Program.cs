using Netduino.Foundation.IC;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace ShiftRegisterTest
{
    public class Program
    {
        public static void Main()
        {
            ShiftRegister74595 shiftRegister = new ShiftRegister74595(8, Pins.GPIO_PIN_D8);
            while (true)
            {
                for (byte index = 0; index <= 7; index++)
                {
                    shiftRegister[index] = true;
                    shiftRegister.LatchData();
                    Thread.Sleep(500);
                    shiftRegister[index] = false;
                }
            }
        }
    }
}
