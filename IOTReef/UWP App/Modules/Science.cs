using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UWP_App.Helpers;
using UWP_App.Models;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UWP_App.Modules
{
    public class Science : ModuleBase
    {
        private DispatcherTimer dataUpdate;
        private DispatcherTimer dataToCloud;
        private List<ScienceModuleData> DataToSend;
        private IOTHelper iothelp;

        public Science(string vid, string pid, Dictionary<string, Outlet> dict) : base(vid, pid, dict)
        {
            iothelp = new IOTHelper();

            DataToSend = new List<ScienceModuleData>();

            Device.StringMessageReceived += Device_StringMessageReceived;

            dataUpdate = new DispatcherTimer();
            dataUpdate.Interval = new TimeSpan(0, 0, 5);
            dataUpdate.Tick += DataUpdate_Tick;
            dataUpdate.Start();

            dataToCloud = new DispatcherTimer();
            dataToCloud.Interval = new TimeSpan(0, 0, 30);
            dataToCloud.Tick += DataToCloud_TickAsync;
            dataToCloud.Start();
        }

        private async void DataToCloud_TickAsync(object sender, object e)
        {
            try
            {
                if(await iothelp.SendData(DataToSend))
                {
                    //succesfull send, clear the list
                    DataToSend.Clear();
                }

            }
            catch (Exception ex)
            {
                //var msg = new MessageDialog("Unknown Error sending data to cloud: " + ex.ToString());
                //msg.Commands.Add(new UICommand("Close"));
            }
        }

        private void DataUpdate_Tick(object sender, object e)
        {
            byte PH_Query = 0x44;
            Firmata.sendSysex(PH_Query, new byte[] { }.AsBuffer());
            Firmata.flush();
        }

        private void Device_StringMessageReceived(string message)
        {
            ScienceModuleData deserialized = new ScienceModuleData();

            if (CurrentData == null)
            {
                CurrentData = new ScienceModuleData();
            }

            deserialized = JsonConvert.DeserializeObject<ScienceModuleData>(message);
            CurrentData = deserialized;
            DataToSend.Add(deserialized);

            CurrentData.TimeRead = DateTime.Now;

            foreach (var outlet in OutletDict)
            {
                outlet.Value.CheckTriggers(CurrentData);
            }
        }
    }
}
