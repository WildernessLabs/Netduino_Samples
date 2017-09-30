using System.Threading;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Netduino.Foundation.Sensors.Temperature.Analog;

namespace NFAnalogTemperatureSensorTest
{
    public class Program
    {
        public static void Main()
        {
            Debug.Print("** Read TMP36 **");
            var tmp36 = new AnalogTemperatureSensor(AnalogChannels.ANALOG_PIN_A0, AnalogTemperatureSensor.SensorType.TMP36);
            while (true)
            {
                Debug.Print("Reading: " + tmp36.Temperature.ToString("f2"));
                Thread.Sleep(5000);
            }
        }
    }
}
