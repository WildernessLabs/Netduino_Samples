using System.Threading;

namespace Blinky_MCP23008
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