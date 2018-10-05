using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using System.Text;
using System.IO;
using System.Collections;

namespace PlantHost
{
    public static class MicroSDController
    {
        public static void Save(ArrayList humidityLogs)
        {
            var volume = new VolumeInfo("SD");
            if (volume != null)
            {
                var path = Path.Combine("SD", "test.txt");
                string json = Json.NETMF.JsonSerializer.SerializeObject(humidityLogs);
                File.WriteAllBytes(path, Encoding.UTF8.GetBytes(json));
                volume.FlushAll();
            }
            else
            {
                Debug.Print("There doesn't appear to be an SD card inserted");
            }
        }

        public static ArrayList Load()
        {
            var volume = new VolumeInfo("SD");
            if (volume != null)
            {
                var path = Path.Combine("SD", "test.txt");
                string json = new string(Encoding.UTF8.GetChars(File.ReadAllBytes(path)));
                var logs = Json.NETMF.JsonSerializer.DeserializeString(json);

                var humidityLogs = new ArrayList();
                foreach(Hashtable item in logs as ArrayList)
                {
                    humidityLogs.Add(new HumidityLog()
                    {
                        Date = item["Date"].ToString(),
                        Humidity = int.Parse(item["Humidity"].ToString())
                    });
                }

                volume.FlushAll();

                return humidityLogs;
            }
            else
            {
                Debug.Print("There doesn't appear to be an SD card inserted");
            }

            return new ArrayList();
        }
    }
}
