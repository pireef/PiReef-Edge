using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_App.Models;

namespace UWP_App.Helpers
{
    public class IOTHelper
    {
        private string HostName;
        private string DeviceName;
        private string DeviceKey;

        private DeviceClient client;

        public IOTHelper()
        {
            HostName = "IOT-Reef.azure-devices.net";
            DeviceName = "DevelopmentDevice";
            DeviceKey = "GTO6JqpfUNkDSD1JmSM1KYUr4VwwcEU2YJMEifhyFjU=";
            client = DeviceClient.Create(HostName, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceName, DeviceKey), Microsoft.Azure.Devices.Client.TransportType.Http1);
        }

        public void SetUp()
        {
            client = DeviceClient.Create(HostName, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceName, DeviceKey), Microsoft.Azure.Devices.Client.TransportType.Http1);
        }

        public async Task<bool> SendData(List<ScienceModuleData> data)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(serializedData);
                var message = new Message(bytes);
                await client.SendEventAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                //var msg = new MessageDialog("Unknown Error sending data to cloud: " + ex.ToString());
                //msg.Commands.Add(new UICommand("Close"));
                return false;
            }

        }
    }
}
