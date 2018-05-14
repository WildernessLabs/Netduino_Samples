using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace ChickenCoop.SunsetSunrise
{
    public class Program
    {
        static App app = null;

        public static void Main()
        {
            app = new App();
            app.Run();
        }
    }
}