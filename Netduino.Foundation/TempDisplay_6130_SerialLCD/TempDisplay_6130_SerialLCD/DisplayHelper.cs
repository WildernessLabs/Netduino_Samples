using System;
using Microsoft.SPOT;
using F = Netduino.Foundation;

namespace TempDisplay_6130_SerialLCD
{
    public static class DisplayHelper
    {
        public static void WriteLine(byte lineRowNumber, string text, F.Displays.SerialLCD display, DisplayConfig config)
        {
            if(text.Length > config.Width)
            {
                throw new Exception("number characters must be <= columns");
            }

            // clear the line
            ClearLine(lineRowNumber, display, config);

            // write the line
            display.SetCursorPosition(0, lineRowNumber);
            display.DisplayText(text);
        }

        public static void ClearLine(byte lineRowNumber, F.Displays.SerialLCD display, DisplayConfig config)
        {
            // clear the line
            display.SetCursorPosition(0, lineRowNumber);
            char[] clearChars = new char[config.Width];
            for (int i = 0; i < config.Width; i++)
            {
                clearChars[i] = ' ';
            }
            string clearString = new string(clearChars);
            display.DisplayText(clearString);

        }
    }
}
