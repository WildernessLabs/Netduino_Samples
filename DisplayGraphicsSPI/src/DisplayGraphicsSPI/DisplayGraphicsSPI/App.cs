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
        readonly Color WatchBackgroundColor = Color.White;

        protected int hour, minute, second, tick;
        protected ILI9163 tftDisplay;
        protected GraphicsLibrary display;

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

        protected void DrawShapes()
        {
            display.Clear(true);

            display.DrawRectangle(3,  3, 26, 26, Color.Red, true); 
            display.DrawRectangle(37, 6, 20, 20, Color.Red, true);
            display.DrawRectangle(65, 9, 14, 14, Color.Red, true);
            display.DrawRectangle(87, 12, 8, 8, Color.Red, true);
            display.DrawRectangle(103, 15, 2, 2, Color.Red, true);
            display.Show();
            //Thread.Sleep(DRAW_TIME);

            display.DrawRectangle(3,  32, 26, 26, Color.Red);
            display.DrawRectangle(37, 35, 20, 20, Color.Red);
            display.DrawRectangle(65, 38, 14, 14, Color.Red);
            display.DrawRectangle(87, 41, 8, 8, Color.Red);
            display.DrawRectangle(103, 44, 2, 2, Color.Red);
            display.Show();
            //Thread.Sleep(DRAW_TIME);

            display.DrawCircle(16, 73, 13, Color.Green, true);
            display.DrawCircle(47, 73, 10, Color.Green, true);
            display.DrawCircle(72, 73, 7, Color.Green, true);
            display.DrawCircle(91, 73, 4, Color.Green, true);
            display.DrawCircle(104, 73, 1, Color.Green, true);
            display.Show();
            //Thread.Sleep(DRAW_TIME);

            display.DrawCircle(16, 103, 13, Color.Green);
            display.DrawCircle(47, 103, 10, Color.Green);
            display.DrawCircle(72, 103, 7, Color.Green);
            display.DrawCircle(91, 103, 4, Color.Green);
            display.DrawCircle(104, 103, 1, Color.Green);
            display.Show();
            //Thread.Sleep(DRAW_TIME);

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
            Thread.Sleep(5000);
        }

        protected void DrawTexts()
        {
            display.Clear(true);

            display.CurrentFont = new Font8x12();
            display.DrawText(4, 4, "abcdefghijklm", Color.White);
            display.DrawText(4, 18, "nopqrstuvwxyz", Color.White);
            display.DrawText(4, 32, "`1234567890-=", Color.White);
            display.DrawText(4, 46, "~!@#$%^&*()_+", Color.White);
            display.DrawText(4, 60, "[]\\;',./", Color.White);
            display.DrawText(4, 74, "{}|:\"<>?", Color.White);
            display.DrawText(4, 88, "ABCDEFGHIJKLM", Color.White);
            display.DrawText(4, 102, "NOPQRSTUVWXYZ", Color.White);

            display.CurrentFont = new Font4x8();
            display.DrawText(4, 116, "ABCDEFGHIJKLMNOPQRSTUVWXYZ", Color.White);
            display.DrawText(4, 126, "abcdefghijklmnopqrstuvwxyz", Color.White);
            display.DrawText(4, 136, "01234567890!@#$%^&*()_+-=", Color.White);
            display.DrawText(4, 146, "\\|;:'\",<.>/?[]{}", Color.White);

            display.Show();
            Thread.Sleep(5000);
        }

        protected void DrawClock()
        {
            hour = 8;
            minute = 54;

            DrawWatchFace();

            while (true)
            {
                tick++;
                Thread.Sleep(DRAW_TIME * 2);
                UpdateClock(second: tick % 60);
            }
        }
        protected void DrawWatchFace()
        {
            display.Clear();

            int xCenter = 64;
            int yCenter = 80;
            int x, y;

            display.CurrentFont = new Font8x12();
            display.DrawText(4, 4, "Wilderness Labs");
            display.DrawText(7, 144, "Netduino Clock");
            
            display.DrawCircle(xCenter, yCenter, 56, WatchBackgroundColor, true);

            for (int i = 0; i < 60; i++)
            {
                x = (int)(xCenter + 50 * System.Math.Sin(i * System.Math.PI / 30));
                y = (int)(yCenter - 50 * System.Math.Cos(i * System.Math.PI / 30));

                if (i % 5 == 0)
                    display.DrawCircle(x, y, 2, Color.Black, true);
                else
                    display.DrawPixel(x, y, Color.Black);
            }
        }
        protected void UpdateClock(int second = 0)
        {
            int xCenter = 64, yCenter = 80;
            int x, y, xT, yT;

            if (second == 0)
            {
                minute++;
                if (minute == 60)
                {
                    minute = 0;
                    hour++;
                    if (hour == 12)
                    {
                        hour = 0;
                    }
                }
            }

            //remove previous hour
            int previousHour = (hour - 1) < -1 ? 11 : (hour - 1);
            x = (int)(xCenter + 23 * System.Math.Sin(previousHour * System.Math.PI / 6));
            y = (int)(yCenter - 23 * System.Math.Cos(previousHour * System.Math.PI / 6));
            xT = (int)(xCenter + 3 * System.Math.Sin((previousHour - 3) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((previousHour - 3) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, WatchBackgroundColor);
            xT = (int)(xCenter + 3 * System.Math.Sin((previousHour + 3) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((previousHour + 3) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, WatchBackgroundColor);

            //current hour
            x = (int)(xCenter + 23 * System.Math.Sin(hour * System.Math.PI / 6));
            y = (int)(yCenter - 23 * System.Math.Cos(hour * System.Math.PI / 6));
            xT = (int)(xCenter + 3 * System.Math.Sin((hour - 3) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((hour - 3) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, Color.Black);
            xT = (int)(xCenter + 3 * System.Math.Sin((hour + 3) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((hour + 3) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, Color.Black);

            //remove previous minute
            int previousMinute = minute - 1 < -1 ? 59 : (minute - 1);
            x = (int)(xCenter + 35 * System.Math.Sin(previousMinute * System.Math.PI / 30));
            y = (int)(yCenter - 35 * System.Math.Cos(previousMinute * System.Math.PI / 30));
            xT = (int)(xCenter + 3 * System.Math.Sin((previousMinute - 15) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((previousMinute - 15) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, WatchBackgroundColor);
            xT = (int)(xCenter + 3 * System.Math.Sin((previousMinute + 15) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((previousMinute + 15) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, WatchBackgroundColor);

            //current minute
            x = (int)(xCenter + 35 * System.Math.Sin(minute * System.Math.PI / 30));
            y = (int)(yCenter - 35 * System.Math.Cos(minute * System.Math.PI / 30));
            xT = (int)(xCenter + 3 * System.Math.Sin((minute - 15) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((minute - 15) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, Color.Black);
            xT = (int)(xCenter + 3 * System.Math.Sin((minute + 15) * System.Math.PI / 6));
            yT = (int)(yCenter - 3 * System.Math.Cos((minute + 15) * System.Math.PI / 6));
            display.DrawLine(xT, yT, x, y, Color.Black);

            //remove previous second
            int previousSecond = second - 1 < -1 ? 59 : (second - 1);
            x = (int)(xCenter + 40 * System.Math.Sin(previousSecond * System.Math.PI / 30));
            y = (int)(yCenter - 40 * System.Math.Cos(previousSecond * System.Math.PI / 30));
            display.DrawLine(xCenter, yCenter, x, y, WatchBackgroundColor);

            //current second
            x = (int)(xCenter + 40 * System.Math.Sin(second * System.Math.PI / 30));
            y = (int)(yCenter - 40 * System.Math.Cos(second * System.Math.PI / 30));
            display.DrawLine(xCenter, yCenter, x, y, Color.Red);

            display.Show();
        }

        public void Run()
        {
            DrawShapes();
            
            DrawTexts();

            DrawClock();
        }
    }
}
