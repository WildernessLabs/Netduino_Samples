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
            ONBOARD_LED = new OutputPort(Pins.ONBOARD_LED, false);
            GPIO_PIN_D1 = new OutputPort(Pins.GPIO_PIN_D1, false);
        }

        public static OutputPort ONBOARD_LED;
        public static OutputPort GPIO_PIN_D1;
    }
}
