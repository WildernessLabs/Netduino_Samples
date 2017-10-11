using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace Connected_Coffee_SimpleControl
{
    public class Program
	{
		public static void Main()
		{
			// create an output port (a port that can be written to) and connect it to Digital Pin 2
			OutputPort relay = new OutputPort(Pins.GPIO_PIN_D2, false);

			// run forever
			while (true)
			{
				relay.Write(true); // turn on the relay
				Thread.Sleep(5000); // Leave on for 5 seconds
				relay.Write(false); // turn off the relay
				Thread.Sleep(5000); // leave off for 5 seconds
			}
		}
	}
}