using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
//using IOTReef_HubModule.Models;
using Microsoft.Maker.Firmata;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
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
using UWP_App.Scheduling;

namespace UWP_App.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        DispatcherTimer UIUpdateTimer;

        public MainPage()
        {
            InitializeComponent();
            //ShellPage.currentData.PropertyChanged += DataChanged;

            UIUpdateTimer = new DispatcherTimer();
            UIUpdateTimer.Interval = new TimeSpan(0, 0, 5);
            UIUpdateTimer.Tick += UIUpdateTick;
            UIUpdateTimer.Start();
        }

        private void UIUpdateTick(object sender, object e)
        {
            UpdateUI();
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

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        //private void DataChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    UpdateUI();
        //}

        private void UpdateUI()
        {
            lblTemp.Text = ShellPage.currentData.Temp.ToString();
            lblPH.Text = ShellPage.currentData.PH.ToString();
        }
    }
}
