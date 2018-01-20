using System.Threading;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors;
using F = Netduino.Foundation;

namespace TempDisplay_6130_SerialLCD
{
    public class Program
    {
        public static void Main()
        {
            // initialize our network
            App app = new App();
            app.Run();

            while (app.IsRunning)
            {
                Thread.Sleep(10000);
            }
        }

        public class App
        {
            F.Sensors.Atmospheric.HIH6130 _hih = new F.Sensors.Atmospheric.HIH6130();
            F.Displays.SerialLCD _lcd = new F.Displays.SerialLCD();
            DisplayConfig _displayConfig = new DisplayConfig() { Width = 20, Height = 4 };

            public bool IsRunning { get; set; }

            public void Run()
            {
                this.IsRunning = true;

                _hih.TemperatureChanged += (object sender, SensorFloatEventArgs e) =>
                {
                    DisplayTemperature(e.CurrentValue);
                };

                _lcd.Clear();

                // display the initial stuff
                DisplayTemperature(_hih.Temperature);

            }

            public void DisplayTemperature(float value)
            {
                char degree = System.Convert.ToChar(223);
                string temp = value.ToString("N2");
                string text = ("Temp: " + temp + degree + "C");
                DisplayHelper.WriteLine(0, text, _lcd, _displayConfig);
            }

            public void DisplayHumidity(float value)
            { }
        }
    }
}