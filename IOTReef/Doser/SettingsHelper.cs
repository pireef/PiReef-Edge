using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Doser
{
    static class SettingsHelper
    {
        public static DosingSettings ReadSettings()
        {
            DosingSettings stn;
            StorageFolder settingsFolder = ApplicationData.Current.LocalFolder;
            string settingsFile = settingsFolder.Path + "\\dosersettings.json";

            using (StreamReader rdr = File.OpenText(settingsFile))
            {
                string s = rdr.ReadToEnd();
                stn = JsonConvert.DeserializeObject<DosingSettings>(s);

            }
            return stn;
        }

        public static void WriteSettings(DosingSettings newSettings)
        {
            StorageFolder settingsFolder = ApplicationData.Current.LocalFolder;
            string settingsFile = settingsFolder.Path + "\\dosersettings.json";

            using (StreamWriter file = File.CreateText(settingsFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, newSettings);
            }
        }
    }
}
