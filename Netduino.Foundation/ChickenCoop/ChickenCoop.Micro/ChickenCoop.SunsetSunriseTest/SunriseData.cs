using System;

namespace ChickenCoop.Micro.SunsetSunrise
{
    public class SunriseData
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }

        public static SunriseData Unknown = new SunriseData()
        {
            Longitude = double.MaxValue,
            Latitude = double.MaxValue,
            Sunrise = DateTime.MaxValue,
            Sunset = DateTime.MaxValue
        };

        public override string ToString()
        {
            return "Sunrise: " + Sunrise + " Sunset: " + Sunset + " Lat: " + Latitude + " Long: " + Longitude;
        }
    }
}