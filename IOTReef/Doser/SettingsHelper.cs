using Newtonsoft.Json;
using System;
using System.IO;

namespace Doser
{
    static class SettingsHelper
    {
        public static DosingSettings ReadSettings()
        {
            DosingSettings stn;
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string settingsFile = path + "/dosersettings.json";

            using (StreamReader rdr = File.OpenText(settingsFile))
            {
                string s = rdr.ReadToEnd();
                stn = JsonConvert.DeserializeObject<DosingSettings>(s);
            }
            return stn;
        }

        public static void WriteSettings(DosingSettings newSettings)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string settingsFile = path + "/dosersettings.json";

            using (StreamWriter file = File.CreateText(settingsFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, newSettings);
            }
        }
    }
}
