using IOTReef_HubModule.Models;
using Microsoft.Maker.Firmata;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using Microsoft.Azure.Devices.Client;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOTReef_HubModule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IStream connection;
        RemoteDevice arduino;
        UwpFirmata firmata;

        DispatcherTimer getDatatimer;
        DispatcherTimer sendDataTimer;

        ScienceModuleData currentData;

        private DeviceClient client;
        private string IOTHostName = "IOT-Reef.azure-devices.net";
        private string IOTDeviceName = "DevelopmentDevice";
        private string IOTDeviceKey = "GTO6JqpfUNkDSD1JmSM1KYUr4VwwcEU2YJMEifhyFjU=";

        public MainPage()
        {
            this.InitializeComponent();

            client = DeviceClient.Create(IOTHostName, new DeviceAuthenticationWithRegistrySymmetricKey(IOTDeviceName, IOTDeviceKey), Microsoft.Azure.Devices.Client.TransportType.Http1);

            connection = new UsbSerial("VID_2341", "PID_0043");
            firmata = new UwpFirmata();
            arduino = new RemoteDevice(firmata);

            firmata.begin(connection);
            connection.begin(57600, SerialConfig.SERIAL_8N1);

            arduino.DeviceReady += SienceModuleReady;
            arduino.DeviceConnectionFailed += ScienceDeviceConnectionFail;
            arduino.StringMessageReceived += ScienceDataReceived;

            getDatatimer = new DispatcherTimer();
            getDatatimer.Interval = new TimeSpan(0, 0, 5);
            getDatatimer.Tick += getDataTick;
            getDatatimer.Start();

            sendDataTimer = new DispatcherTimer();
            sendDataTimer.Interval = new TimeSpan(0, 0, 30);
            sendDataTimer.Tick += sendDataTickAsync;
            sendDataTimer.Start();
        }

        private async void sendDataTickAsync(object sender, object e)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(currentData);
                var bytes = Encoding.UTF8.GetBytes(serializedData);
                var message = new Message(bytes);
                await client.SendEventAsync(message);

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                lblMessages.Text = "Last Data Sent to Cloud: " + currentData.TimeRead.ToString();
                //lblPH.Text = currentData.PH.ToString();
                //lblTemp.Text = currentData.Temp.ToString();
            });
            }
            catch (Exception ex)
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    lblMessages.Text = "Unhandled Exception: " + ex.ToString();
                });
            }


        }

        private void getDataTick(object sender, object e)
        {
            byte PH_Query = 0x44;
            firmata.sendSysex(PH_Query, new byte[] { }.AsBuffer());
            firmata.flush();
        }

        private void ScienceDataReceived(string message)
        {
            ScienceModuleData deserialized = new ScienceModuleData();

            if(currentData == null)
            {
                currentData = new ScienceModuleData();
            }

            deserialized = JsonConvert.DeserializeObject<ScienceModuleData>(message);
            currentData = deserialized;
            currentData.TimeRead = DateTime.Now;
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
                {
                    lblMessages.Text = "Last Data Update: " + currentData.TimeRead.ToString();
                    lblPH.Text = currentData.PH.ToString();
                    lblTemp.Text = currentData.Temp.ToString();
                });
        }

        private void ScienceDeviceConnectionFail(string message)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, ()=>{lblMessages.Text = "Science Module Connection Failed!";});
        }

        private void SienceModuleReady()
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, ()=>{lblMessages.Text = "Science Module Ready!";});
        }
    }
}
