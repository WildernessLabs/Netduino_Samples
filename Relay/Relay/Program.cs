using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace Relay
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
                relay.Write(true); // turn on the LED
                Thread.Sleep(500); // sleep for 1/2 second
                relay.Write(false); // turn off the relay
                Thread.Sleep(500); // sleep for 1/2 second
            }
        }
    }
}