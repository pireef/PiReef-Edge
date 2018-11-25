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
using Windows.Storage;
using FluentScheduler;
using IOTReef_HubModule.Scheduling;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOTReef_HubModule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IStream s_connection;
        RemoteDevice s_arduino;
        UwpFirmata s_firmata;

        IStream p_connection;
        RemoteDevice p_arduino;
        UwpFirmata p_firmata;

        DispatcherTimer getDatatimer;
        DispatcherTimer sendDataTimer;

        ScienceModuleData currentData;

        private DeviceClient client;
        private string IOTHostName = "IOT-Reef.azure-devices.net";
        private string IOTDeviceName = "DevelopmentDevice";
        private string IOTDeviceKey = "GTO6JqpfUNkDSD1JmSM1KYUr4VwwcEU2YJMEifhyFjU=";

        Dictionary<string, Outlet> outletDict; //outlets so we can call them by "name"
        Dictionary<int, byte> pinNumDict; //physical pin mappings plug number -> pin number this shouldn't change
        //Dictionary<int, string> nameDict; //software plug mappings plug number -> name

        public MainPage()
        {
            this.InitializeComponent();

            client = DeviceClient.Create(IOTHostName, new DeviceAuthenticationWithRegistrySymmetricKey(IOTDeviceName, IOTDeviceKey), Microsoft.Azure.Devices.Client.TransportType.Http1);

            s_connection = new UsbSerial("VID_2341", "PID_0043");
            s_firmata = new UwpFirmata();
            s_arduino = new RemoteDevice(s_firmata);

            s_firmata.begin(s_connection);
            s_connection.begin(57600, SerialConfig.SERIAL_8N1);

            s_arduino.DeviceReady += SienceModuleReadyAsync;
            s_arduino.DeviceConnectionFailed += ScienceDeviceConnectionFailAsync;
            s_arduino.StringMessageReceived += ScienceDataReceivedAsync;

            p_connection = new UsbSerial("VID_0403", "PID_6001");
            p_firmata = new UwpFirmata();
            p_arduino = new RemoteDevice(p_firmata);

            p_firmata.begin(p_connection);
            p_connection.begin(57600, SerialConfig.SERIAL_8N1);

            p_arduino.DeviceReady += PowerModuleReadyAsync;
            p_arduino.DeviceConnectionFailed += PowerConnectionFailAsync;

            getDatatimer = new DispatcherTimer();
            getDatatimer.Interval = new TimeSpan(0, 0, 5);
            getDatatimer.Tick += getDataTick;
            getDatatimer.Start();

            sendDataTimer = new DispatcherTimer();
            sendDataTimer.Interval = new TimeSpan(0, 0, 30);
            sendDataTimer.Tick += sendDataTickAsync;
            sendDataTimer.Start();
        }

        private async void PowerConnectionFailAsync(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { lblMessages.Text = "Power Module Connection Failed!"; });
        }

        //Ok here's what we're doing. 
        //1. Try and load the outlet information from the settings file
        //2. Problem is on a new debug session it's wiping that file.
        //3. If successful it will load the information on what plugs control what, if not,
        //we load the default file from the assets folder
        //4. Call method which basically is an after-thought constructor, sets the arduino, and the current state
        //that doesn't get called during the DesrializeObject call. (Look into JsonConstructor attibute)
        private async void PowerModuleReadyAsync()
        {
            pinNumDict = new Dictionary<int, byte>
            {
                { 1, 2 },
                { 2, 3 },
                { 3, 4 },
                { 4, 5 },
                { 5, 6 },
                { 6, 7 },
                { 7, 8 },
                { 8, 9 },
                { 9, 10},
                {10, 11},
                {11, 12},
                {12, 13}
            };

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile outletfile = await localFolder.GetFileAsync("dictionarysettings.txt");
                string outletState = await FileIO.ReadTextAsync(outletfile);
                outletDict = JsonConvert.DeserializeObject<Dictionary<string, Outlet>>(outletState);

                foreach (var plug in outletDict)
                {
                    plug.Value.AfterDataConst(p_arduino);
                }
            }
            catch(FileNotFoundException fnfex)
            {
                StorageFolder localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile defaultFile = await localFolder.GetFileAsync("Assets\\OutletDefault.txt");
                string outletState = await FileIO.ReadTextAsync(defaultFile);
                outletDict = JsonConvert.DeserializeObject<Dictionary<string, Outlet>>(outletState);
                foreach(var plug in outletDict)
                {
                    plug.Value.AfterDataConst(p_arduino);
                }
                string serialized = JsonConvert.SerializeObject(outletDict);
                localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync("dictionarysettings.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, serialized);
            }
            catch (Exception ex)
            {
                var msg = new MessageDialog("Unknown Error reading settings files: " + ex.ToString());
                msg.Commands.Add(new UICommand("Close"));
            }
            JobManager.Initialize(new FluentRegistry(outletDict));
            foreach(var plug in outletDict)
            {
                plug.Value.PowerUpRecovery();
            }

        }

        private async void sendDataTickAsync(object sender, object e)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(currentData);
                var bytes = Encoding.UTF8.GetBytes(serializedData);
                var message = new Message(bytes);
                await client.SendEventAsync(message);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 lblMessages.Text = "Last Data Sent to Cloud: " + currentData.TimeRead.ToString();
             });
            }
            catch (Exception ex)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                 {
                     lblMessages.Text = "Unhandled Exception: " + ex.ToString();
                 });
            }


        }

        private void getDataTick(object sender, object e)
        {
            byte PH_Query = 0x44;
            s_firmata.sendSysex(PH_Query, new byte[] { }.AsBuffer());
            s_firmata.flush();
        }

        private async void ScienceDataReceivedAsync(string message)
        {
            ScienceModuleData deserialized = new ScienceModuleData();

            if(currentData == null)
            {
                currentData = new ScienceModuleData();
            }

            deserialized = JsonConvert.DeserializeObject<ScienceModuleData>(message);
            currentData = deserialized;
            currentData.TimeRead = DateTime.Now;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                 {
                     lblMessages.Text = "Last Data Update: " + currentData.TimeRead.ToString();
                     lblPH.Text = currentData.PH.ToString();
                     lblTemp.Text = currentData.Temp.ToString();
                 });
            foreach(var outlet in outletDict)
            {
                outlet.Value.CheckTriggers(currentData);
            }
        }

        private async void ScienceDeviceConnectionFailAsync(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { lblMessages.Text = "Science Module Connection Failed!"; });
        }

        private async void SienceModuleReadyAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { lblMessages.Text = "Science Module Ready!"; });
        }
    }
}
