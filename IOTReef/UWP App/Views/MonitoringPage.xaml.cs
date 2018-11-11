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
        //private List<Measurement> measurements;
        private DispatcherTimer dataUpdateTimer;

        public MonitoringPage()
        {
            InitializeComponent();
            measurements = UpdateList();

            dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Interval = new TimeSpan(0, 0, 10);
            dataUpdateTimer.Tick += DataUpdateTimer_Tick;
            dataUpdateTimer.Start();

        }

        private void DataUpdateTimer_Tick(object sender, object e)
        {
            measurements = UpdateList();
            //Both of those work, but that's stupid...
            Bindings.Update();
            //thegrid.ItemsSource = measurements;
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

        private ObservableCollection<Measurement> UpdateList()
        {
            ObservableCollection<Measurement> theList = new ObservableCollection<Measurement>();

            Measurement temp = new Measurement("ms-appx:///Assets/Temp-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.Temp, "Temperature");
            Measurement ph = new Measurement("ms-appx:///Assets/PH-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.PH, "PH");
            Measurement sal = new Measurement("ms-appx:///Assets/Salinity-Icon.png", ShellPage.currentData.TimeRead, ShellPage.currentData.Salinity, "Salinity");

            theList.Add(temp);
            theList.Add(ph);
            theList.Add(sal);

            return theList;
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
