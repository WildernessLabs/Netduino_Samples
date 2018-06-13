using System.Threading;

namespace ChickenCoop.Micro
{
    public class Program
    {
        public static App App = null;

        public static void Main()
        {
            App = new App();
            App.Run();

            Thread.Sleep(Timeout.Infinite);
        }

    }
}
