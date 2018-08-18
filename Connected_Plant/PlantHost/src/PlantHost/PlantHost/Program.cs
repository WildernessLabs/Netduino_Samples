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
            App app = new App();
            app.Run();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}