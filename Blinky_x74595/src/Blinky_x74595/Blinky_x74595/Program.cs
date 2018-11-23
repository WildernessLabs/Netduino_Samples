using System.Threading;

namespace Blinky_x74595
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