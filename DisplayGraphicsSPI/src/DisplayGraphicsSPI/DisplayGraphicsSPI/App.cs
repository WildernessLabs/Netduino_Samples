using Netduino.Foundation.Displays;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Hardware;
using Netduino.Foundation;
using System.Threading;

namespace DisplayGraphicsSPI
{
    public class App
    {
        const int DRAW_TIME = 500;
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
            display.DrawRectangle(37, 6, 20, 20, Color.Red, true);
            display.DrawRectangle(65, 9, 14, 14, Color.Red, true);
            display.DrawRectangle(87, 12, 8, 8, Color.Red, true);
            display.DrawRectangle(103, 15, 2, 2, Color.Red, true);
            display.Show();
            Thread.Sleep(DRAW_TIME);

            display.DrawRectangle(3,  32, 26, 26, Color.Red);
            display.DrawRectangle(37, 35, 20, 20, Color.Red);
            display.DrawRectangle(65, 38, 14, 14, Color.Red);
            display.DrawRectangle(87, 41, 8, 8, Color.Red);
            display.DrawRectangle(103, 44, 2, 2, Color.Red);
            display.Show();
            Thread.Sleep(DRAW_TIME);

            display.DrawCircle(16, 73, 13, Color.Green, true);
            display.DrawCircle(47, 73, 10, Color.Green, true);
            display.DrawCircle(72, 73, 7, Color.Green, true);
            display.DrawCircle(91, 73, 4, Color.Green, true);
            display.DrawCircle(104, 73, 1, Color.Green, true);
            display.Show();
            Thread.Sleep(DRAW_TIME);

            display.DrawCircle(16, 103, 13, Color.Green);
            display.DrawCircle(47, 103, 10, Color.Green);
            display.DrawCircle(72, 103, 7, Color.Green);
            display.DrawCircle(91, 103, 4, Color.Green);
            display.DrawCircle(104, 103, 1, Color.Green);
            display.Show();
            Thread.Sleep(DRAW_TIME);

            for (int i = 0; i < 9; i++)
                display.DrawHorizontalLine(3, 123 + (i * 4), 26, Color.Blue);

            for (int i = 0; i < 7; i++)
                display.DrawVerticalLine(37 + (i * 4), 123, 33, Color.Blue);

            display.DrawLine(70, 131, 94, 147, Color.Blue);
            display.DrawLine(70, 123, 94, 155, Color.Blue);
            display.DrawLine(78, 123, 86, 155, Color.Blue);
            display.DrawLine(86, 123, 78, 155, Color.Blue);
            display.DrawLine(94, 123, 70, 155, Color.Blue);
            display.DrawLine(94, 131, 70, 147, Color.Blue);
            display.DrawLine(70, 139, 94, 139, Color.Blue);
            display.Show();
            Thread.Sleep(DRAW_TIME);
        }

        protected void ShowTextSample()
        {
            display.Clear(true);

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
            display.DrawText(4, 116, "ABCDEFGHIJKLMNOPQRSTUVWXYZ", Color.SkyBlue);
            display.DrawText(4, 126, "abcdefghijklmnopqrstuvwxyz", Color.SkyBlue);
            display.DrawText(4, 136, "01234567890!@#$%^&*()_+-=", Color.SkyBlue);
            display.DrawText(4, 146, "\\|;:'\",<.>/?[]{}", Color.SkyBlue);

            display.Show();
            Thread.Sleep(20000);
        }

        public void Run()
        {
            ShowShapes();
            ShowTextSample();
        }
    }
}
