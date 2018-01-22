using System.Threading;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Sensors;
using Netduino.Foundation.Displays;
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
            F.Sensors.Atmospheric.HIH6130 _hih = null;
            F.Displays.SerialLCD _lcd = null;
            
            public bool IsRunning { get; set; }

            public App()
            {
                _hih = new F.Sensors.Atmospheric.HIH6130();
                TextDisplayConfig displayConfig = new TextDisplayConfig() { Width = 16, Height = 2 };
                this._lcd = new F.Displays.SerialLCD(displayConfig);
            }

            public void Run()
            {
                this.IsRunning = true;

                // wire up our events
                _hih.TemperatureChanged += (object sender, SensorFloatEventArgs e) => {
                    DisplayTemperature(e.CurrentValue);
                };
                _hih.HumidityChanged += (object sender, SensorFloatEventArgs e) => {
                    DisplayHumidity(e.CurrentValue);
                };

                // clear our screen
                _lcd.Clear();

                // display the initial stuff
                DisplayTemperature(_hih.Temperature);
                DisplayHumidity(_hih.Humidity);
            }

            public void DisplayTemperature(float value)
            {
                char degree = System.Convert.ToChar(223);
                string temp = value.ToString("N2");
                string text = ("Temp: " + temp + degree + "C");
                _lcd.WriteLine(text, 0);
            }

            public void DisplayHumidity(float value)
            {
                string text = ("Humidity: " + value.ToString("N2") + "%");
                _lcd.WriteLine(text, 1);
            }

        }
    }
}