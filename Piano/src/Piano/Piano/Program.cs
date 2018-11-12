using System.Threading;

namespace Piano
{
    public class Program
    {
        public static void Main()
        {
            var app = new App();
            app.Run();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}