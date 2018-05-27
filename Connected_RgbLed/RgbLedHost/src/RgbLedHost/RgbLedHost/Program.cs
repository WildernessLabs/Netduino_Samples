using Microsoft.SPOT;
using Netduino.Foundation;
using System.Threading;
using N = SecretLabs.NETMF.Hardware.Netduino;

namespace RgbLedHost
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
