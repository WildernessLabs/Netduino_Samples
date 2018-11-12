using Netduino.Foundation.Displays;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation;
using System.Threading;

namespace DisplayGraphicsSPI
{
    public class App
    {
        protected ILI9163 tftDisplay;
        static GraphicsLibrary display;

        public App()
        {

            InitializePeripherals();
        }

        protected void InitializePeripherals()
        {
            tftDisplay = new ILI9163
            (
                chipSelectPin: Pins.GPIO_PIN_D3,
                dcPin: Pins.GPIO_PIN_D7,
                resetPin: Pins.GPIO_PIN_D6,
                width: 128,
                height: 160,
                spiModule: SPI.SPI_module.SPI1,
                speedKHz: 15000
            );

            tftDisplay.ClearScreen(31);
            tftDisplay.Refresh();

            display = new GraphicsLibrary(tftDisplay);
        }

        protected void ShowShapes()
        {
            display.Clear(true);

            display.DrawRectangle(3,  3, 26, 26, Color.Red, true);
            display.DrawRectangle(32, 3, 20, 20, Color.Red, true);
            display.DrawRectangle(55, 3, 14, 14, Color.Red, true);
            display.DrawRectangle(72, 3, 8, 8, Color.Red, true);
            display.DrawRectangle(83, 3, 2, 2, Color.Red, true);
            //display.Show();
            //Thread.Sleep(1000);

            display.DrawRectangle(3,  32, 26, 26, Color.Red);
            display.DrawRectangle(32, 32, 20, 20, Color.Red);
            display.DrawRectangle(55, 32, 14, 14, Color.Red);
            display.DrawRectangle(72, 32, 8, 8, Color.Red);
            display.DrawRectangle(83, 32, 2, 2, Color.Red);
            //display.Show();

            display.DrawCircle(16, 74, 13, Color.Red, true);
            display.DrawCircle(45, 74, 10, Color.Red, true);
            display.DrawCircle(68, 74, 7, Color.Red, true);
            display.DrawCircle(85, 74, 4, Color.Red, true);
            display.DrawCircle(96, 74, 1, Color.Red, true);
            display.Show();

            //display.Clear(true);
            //display.DrawCircle(63, 31, 20, true);
            //display.DrawCircle(63, 15, 5, true);
            //display.Show();
            //Thread.Sleep(1000);

            //display.Clear(true);
            //display.DrawRectangle(20, 20, 60, 40);
            //display.DrawRectangle(20, 10, 60, 30);
            //display.Show();
            //Thread.Sleep(1000);

            //display.Clear(true);
            //display.DrawRectangle(30, 10, 50, 40);
            //display.DrawRectangle(50, 10, 50, 20);
            //display.Show();
            //Thread.Sleep(1000);
        }

        protected void ShowTextSample()
        {
            display.CurrentFont = new Font8x12();
            display.DrawText(4, 4, "abcdefghijklm", Color.SkyBlue);
            display.DrawText(4, 18, "nopqrstuvwxyz", Color.SkyBlue);
            display.DrawText(4, 32, "`1234567890-=", Color.SkyBlue);
            display.DrawText(4, 46, "~!@#$%^&*()_+", Color.SkyBlue);
            display.DrawText(4, 60, "[]\\;',./", Color.SkyBlue);
            display.DrawText(4, 74, "{}|:\"<>?", Color.SkyBlue);
            display.DrawText(4, 88, "ABCDEFGHIJKLM", Color.SkyBlue);
            display.DrawText(4, 102, "NOPQRSTUVWXYZ", Color.SkyBlue);

            display.CurrentFont = new Font4x8();
            display.DrawText(4, 116, "ABCDEFGHIJKLMNOPQRSTUVWXYZ", Color.White);
            display.DrawText(4, 126, "abcdefghijklmnopqrstuvwxyz", Color.White);
            display.DrawText(4, 136, "01234567890!@#$%^&*()_+-=", Color.White);
            display.DrawText(4, 146, "\\|;:'\",<.>/?[]{}", Color.White);

            display.Show();
            Thread.Sleep(20000);
        }

        public void Run()
        {
            ShowShapes();
            //ShowTextSample();
        }
    }
}
