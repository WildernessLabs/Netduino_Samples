﻿using Microsoft.SPOT;
using Netduino.Foundation.Sensors.Barometric;
using System.Threading;

namespace SHT31DTest
{
    public class Program
    {
        public static void Main()
        {
            SHT31D sht31d = new SHT31D();

            Debug.Print("SHT31D Temperature / Humidity Test");
            while (true)
            {
                sht31d.Read();
                Debug.Print("Temperature: " + sht31d.Temperature.ToString("f2") + ", Humidity: " + sht31d.Humidity.ToString("f2"));
                Thread.Sleep(1000);
            }
        }
    }
}
