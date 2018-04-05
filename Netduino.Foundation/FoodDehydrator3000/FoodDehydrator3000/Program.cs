using System.Threading;

namespace FoodDehydrator3000
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