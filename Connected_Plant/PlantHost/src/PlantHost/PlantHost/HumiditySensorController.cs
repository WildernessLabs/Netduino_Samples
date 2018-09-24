using MSH = Microsoft.SPOT.Hardware;
using SLH = SecretLabs.NETMF.Hardware;
using SLHN = SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace PlantHost
{
    public class HumiditySensorController
    {
        protected SLH.AnalogInput _analogPort;
        protected MSH.OutputPort _digitalPort;

        public HumiditySensorController(MSH.Cpu.Pin analogPort, MSH.Cpu.Pin digitalPort)
        {
            _analogPort = new SLH.AnalogInput(analogPort);
            _digitalPort = new MSH.OutputPort(digitalPort, false);
        }

        public float Read()
        {
            int sample;
            float humidity;

            _digitalPort.Write(true);
            Thread.Sleep(5);
            sample = _analogPort.Read();
            _digitalPort.Write(false);

            humidity = 100 - Map(sample, 250, 1023, 0, 100);
            return humidity;
        }

        protected float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            return (((toHigh - toLow) * (value - fromLow)) / (fromHigh - fromLow)) - toLow;
        }
    }
}