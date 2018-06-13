using System.Threading;
using Microsoft.SPOT;
using H = Microsoft.SPOT.Hardware;

namespace ChickenCoop.SunsetSunrise
{
    public class App
    {
        // Pacific daylight savings is -7, PST is -8
        // TODO: pull daylight savings yes/no from web
        public float _utcOffset = -7;

        public App()
        {
            // initialize our network interfaces
            Netduino.Foundation.Network.Initializer.InitializeNetwork();

            // get the current date time from the server
            var dateTime = Netduino.Foundation.NetworkTime.GetNetworkTime((int)_utcOffset); 

            Debug.Print("Current DateTime: " + dateTime.ToString());

            // save the date time
            H.Utility.SetLocalTime(dateTime);

            
        }

        public void Run()
        {
            var sunrise = new SunriseService();

            // portland
            var data = sunrise.GetSunriseSunset(45.5, -122.7);

            var localSunrise = data.Sunrise.AddHours(_utcOffset);
            var localSunset = data.Sunset.AddHours(_utcOffset);

            Debug.Print("Local Sunrise: " + localSunrise.ToString());
            Debug.Print("Local Sunset: " + localSunrise.ToString());

            Debug.Print(data.ToString());

        }
    }
}
