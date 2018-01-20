using System;
using Microsoft.SPOT;
using F = Netduino.Foundation;

namespace TempDisplay_6130_SerialLCD
{
    public static class DisplayHelper
    {
        public static void WriteLine(int lineRowNumber, string text, F.Displays.SerialLCD display, DisplayConfig config)
        {
            if(text.Length > config.Width)
            {
                throw new Exception("number characters must be <= columns");
            }

            // clear the line
            display.SetCursorPosition(0, 0);
            char[] clearChars = new char[config.Width];
            for (int i = 0; i < config.Width; i++)
            {
                clearChars[i] = ' ';
            }
            string clearString = new string(clearChars);
            display.DisplayText(clearString);

            // write the line
            display.SetCursorPosition(0, 0);
            display.DisplayText(text);
        }
    }
}
