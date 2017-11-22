using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace ApplianceHost
{
    static class Ports
    {
        static Ports()
        {
            Led = new OutputPort(Pins.ONBOARD_LED, false);
        }

        public static OutputPort Led;
    }
}
