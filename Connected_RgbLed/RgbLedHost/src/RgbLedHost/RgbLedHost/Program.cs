using System;
using Microsoft.SPOT;
using System.Threading;

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
