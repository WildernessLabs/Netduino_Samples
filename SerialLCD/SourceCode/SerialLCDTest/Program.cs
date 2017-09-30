using System.Threading;
using Netduino.Foundation.Displays;

namespace SerialLCDTest
{
    public class Program
    {
        public static void Main()
        {
            var display = new SerialLCD();
            //
            //  Clear the display ready for the test.
            //
            display.Clear();
            display.SetCursorStyle(SerialLCD.CursorStyle.BlinkingBoxOff);
            display.SetCursorStyle(SerialLCD.CursorStyle.UnderlineOff);
            //
            //  Display some text on the bottom row of a 16x2 LCD.
            //
            display.SetCursorPosition(2, 1);
            display.DisplayText("Hello, world");
            Thread.Sleep(1000);
            //
            //  Now scroll the text off of the display to the left.
            //
            for (int index = 0; index < 16; index++)
            {
                display.ScrollDisplay(SerialLCD.Direction.Left);
                Thread.Sleep(500);
            }
            //
            //  Put some text on the top line of the display (note that the
            //  text is still off to the left of the display).
            //
            display.SetCursorPosition(0, 0);
            display.DisplayText("Scrolling Right");
            Thread.Sleep(500);
            //
            //  Now scroll the text back on to the display from the left to
            //  the right of the display.
            //
            for (int index = 0; index < 16; index++)
            {
                display.ScrollDisplay(SerialLCD.Direction.Right);
                Thread.Sleep(500);
            }
            //
            //  Now put a cursor on the display and move it around.
            //
            display.SetCursorStyle(SerialLCD.CursorStyle.BlinkingBoxOn);
            for (int index = 0; index < 10; index++)
            {
                display.MoveCursor(SerialLCD.Direction.Left);
                Thread.Sleep(200);
            }
            Thread.Sleep(1000);
            display.SetCursorStyle(SerialLCD.CursorStyle.BlinkingBoxOff);
            //
            //  Done.
            //
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
