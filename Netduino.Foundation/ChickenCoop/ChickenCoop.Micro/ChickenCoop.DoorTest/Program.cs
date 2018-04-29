using System;
using System.Threading;
using Microsoft.SPOT;

namespace ChickenCoop.DoorTest
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
