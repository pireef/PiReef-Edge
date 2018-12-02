using Microsoft.Maker.Firmata;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_App.Helpers;
using UWP_App.Models;

namespace UWP_App.Modules
{
    public class ModuleBase
    {
        private IStream connection;
        private RemoteDevice device;
        private UwpFirmata firmata;
        private string vid;
        private string pid;
        private Dictionary<string, Outlet> outletDict;
        private ScienceModuleData currentData;

        public RemoteDevice Device { get => device; set => device = value; }
        public UwpFirmata Firmata { get => firmata; set => firmata = value; }
        public Dictionary<string, Outlet> OutletDict { get => outletDict; set => outletDict = value; }
        public ScienceModuleData CurrentData { get => currentData; set => currentData = value; }

        public ModuleBase(string vid, string pid, Dictionary<string, Outlet> dict)
        {
            this.vid = vid;
            this.pid = pid;
            this.outletDict = dict;

            CurrentData = new ScienceModuleData();

            connection = new UsbSerial(pid, vid);
            Firmata = new UwpFirmata();
            Device = new RemoteDevice(connection);

            Firmata.begin(connection);
            connection.begin(57600, SerialConfig.SERIAL_8N1);

            Device.DeviceReady += Device_DeviceReady;
            Device.DeviceConnectionFailed += Device_DeviceConnectionFailed;
            Device.DeviceConnectionLost += Device_DeviceConnectionLost;            
        }

        private async void LoadOutletDictionary()
        {
            try
            {
                OutletDict = await OutletStorage.ReadOutletDictionaryAsync("dictionarysettings.txt");
            }
            catch(FileNotFoundException fnfex)
            {
                OutletDict = await OutletStorage.ReadDefaultOutletDictionaryAsync();
            }
        }
        public virtual void Device_DeviceConnectionLost(string message)
        {
            //throw new NotImplementedException();
        }

        public virtual void Device_DeviceConnectionFailed(string message)
        {
            //throw new NotImplementedException();
        }

        public virtual void Device_DeviceReady()
        {
            //throw new NotImplementedException();
        }
    }
}
