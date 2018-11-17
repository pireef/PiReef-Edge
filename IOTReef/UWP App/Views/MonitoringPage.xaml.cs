using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UWP_App.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_App.Views
{
    public sealed partial class MonitoringPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<Measurement> measurements;
        private ObservableCollection<OutletVM> outlets;

        private DispatcherTimer dataUpdateTimer;

        public MonitoringPage()
        {
            InitializeComponent();
            measurements = UpdateMeasurements();
            outlets = UpdateOutlets();

            dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Interval = new TimeSpan(0, 0, 10);
            dataUpdateTimer.Tick += DataUpdateTimer_Tick;
            dataUpdateTimer.Start();

        }

        private void DataUpdateTimer_Tick(object sender, object e)
        {
            measurements = UpdateMeasurements();
            outlets = UpdateOutlets();
            //This works, but it's a little stupid...
            Bindings.Update();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private ObservableCollection<OutletVM> UpdateOutlets()
        {
            ObservableCollection<OutletVM> theList = new ObservableCollection<OutletVM>();
            Dictionary<string, Outlet> wholeDict = ShellPage.outletDict;

            foreach(var kvp in wholeDict)
            {
                theList.Add(new OutletVM(kvp.Key, kvp.Value.State));
            }

            return theList;
        }
        private ObservableCollection<Measurement> UpdateMeasurements()
        {
            ObservableCollection<Measurement> theList = new ObservableCollection<Measurement>();

            //Measurement temp = new Measurement("ms-appx:///Assets/Temp-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.Temp, "Temperature");
            //Measurement ph = new Measurement("ms-appx:///Assets/PH-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.PH, "PH");
            //Measurement sal = new Measurement("ms-appx:///Assets/Salinity-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.Salinity, "Salinity");

            theList.Add(new Measurement("ms-appx:///Assets/Temp-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.Temp, "Temperature"));
            theList.Add(new Measurement("ms-appx:///Assets/PH-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.PH, "PH"));
            theList.Add(new Measurement("ms-appx:///Assets/Salinity-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.Salinity, "Salinity"));

            return theList;
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void outletGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var plug = (OutletVM)e.ClickedItem;
        }
    }
}
