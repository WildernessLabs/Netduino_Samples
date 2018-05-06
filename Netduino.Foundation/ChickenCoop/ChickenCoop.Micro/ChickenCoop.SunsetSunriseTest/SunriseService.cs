using Microsoft.SPOT;
using System;
using System.IO;
using System.Net;

/* Uses the free Sunset and sunrise times API
 * A free API that provides sunset and sunrise times for a given latitude and longitude.
 * Please note that attribution is required if you use the API. 
 * 
 * https://api.sunrise-sunset.org
 */
namespace ChickenCoop.SunsetSunrise
{
    public class SunriseService
    {
        const string SunriseSunsetServiceUrl = "https://api.sunrise-sunset.org";
        public SunriseService()
        {
        }

        public SunriseData GetSunriseSunset(double latitude, double longitude)
        {
            var url = SunriseSunsetServiceUrl + "/json?lat=" + latitude + "&lng=" + longitude + "&date=today";

            var json = MakeWebRequest(url);

            if (json == null || json == "")
                return SunriseData.Unknown;

            var sunriseText = GetJsonSubstringValue(json, "sunrise");
            var sunsetText = GetJsonSubstringValue(json, "sunset");

            var result = new SunriseData()
            {
                Latitude = latitude,
                Longitude = longitude,
                Sunrise = TimeStringToDateTime(sunriseText),
                Sunset = TimeStringToDateTime(sunsetText)
            };

            return result;
        }

        DateTime TimeStringToDateTime(string time)
        {
            int hours = 0, minutes = 0, seconds = 0;

            try
            {
                var index = time.IndexOf(":");
                var temp = time.Substring(0, index);

                hours = int.Parse(temp);

                if (hours != 12 && time.IndexOf("PM") != -1)
                    hours += 12;

                index = time.IndexOf(":", index);
                temp = time.Substring(index + 1, 2);

                minutes = int.Parse(temp);

                index = time.IndexOf(":", index + 1);
                temp = time.Substring(index + 1, 2);

                seconds = int.Parse(temp);
            }
            catch { }

            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hours, minutes, seconds);
        }

        string GetJsonSubstringValue(string source, string key)
        {
            var startIndex = source.IndexOf(key);
            startIndex = source.IndexOf("\"", ++startIndex);
            startIndex = source.IndexOf("\"", ++startIndex);
            var endIndex = source.IndexOf("\"", ++startIndex);

            return source.Substring(startIndex, endIndex - startIndex);
        }

        string MakeWebRequest(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                Debug.Print("this is what we got from " + url + ": " + result);
            }

            return result;
        }
    }
}