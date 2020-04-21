using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP_App.Models;
using Windows.Storage;

namespace UWP_App.Helpers
{
    public static class OutletStorage
    {
        public static async Task SaveOutletDictionaryAsync(Dictionary<string, Outlet> newValues, string fileName)
        {
            StorageFolder localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string serialized = JsonConvert.SerializeObject(newValues);
            localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, serialized);
        }

        public static async Task<Dictionary<string, Outlet>> ReadOutletDictionaryAsync(string fileName)
        {
            Dictionary<string, Outlet> readValues;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile outletfile = await localFolder.GetFileAsync(fileName);
            string outletState = await FileIO.ReadTextAsync(outletfile);
            readValues = JsonConvert.DeserializeObject<Dictionary<string, Outlet>>(outletState);

            return readValues;

        }

        public static async Task<Dictionary<string, Outlet>> ReadDefaultOutletDictionaryAsync()
        {
            Dictionary<string, Outlet> readValues;

            StorageFolder localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile defaultFile = await localFolder.GetFileAsync("Assets\\OutletDefault.txt");
            string outletState = await FileIO.ReadTextAsync(defaultFile);
            readValues = JsonConvert.DeserializeObject<Dictionary<string, Outlet>>(outletState);

            return readValues;
        }
    }
}
