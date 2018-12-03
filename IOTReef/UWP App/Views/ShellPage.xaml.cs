using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using UWP_App.Helpers;
using UWP_App.Services;

using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UWP_App.Models;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.Devices.Client;
using UWP_App.Modules;
using System.Threading.Tasks;

namespace UWP_App.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private NavigationViewItem _selected;

        internal static Science sci_mod;
        internal static Power pow_mod;

        private DeviceClient client;
        private string IOTHostName = "IOT-Reef.azure-devices.net";
        private string IOTDeviceName = "DevelopmentDevice";
        private string IOTDeviceKey = "GTO6JqpfUNkDSD1JmSM1KYUr4VwwcEU2YJMEifhyFjU=";

        internal static Dictionary<string, Outlet> outletDict; //outlets so we can call them by "name"

        public NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ShellPage()
        {
            
            InitializeComponent();
            HideNavViewBackButton();
            DataContext = this;
            Initialize();
            
            SetupAsync();
        }

        private async Task SetupAsync()
        {
            client = DeviceClient.Create(IOTHostName, new DeviceAuthenticationWithRegistrySymmetricKey(IOTDeviceName, IOTDeviceKey), Microsoft.Azure.Devices.Client.TransportType.Http1);

            await LoadDictionary();
            sci_mod = new Science("VID_2341", "PID_0043", outletDict);
            pow_mod = new Power("VID_0403", "PID_6001", outletDict);

        }
        private async Task LoadDictionary()
        {            
            try
            {
                outletDict = await OutletStorage.ReadOutletDictionaryAsync("dictionarysettings.txt");
            }
            catch (FileNotFoundException fnfex)
            {
                outletDict = await OutletStorage.ReadDefaultOutletDictionaryAsync();
            }
        }

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.Navigated += Frame_Navigated;
            KeyboardAccelerators.Add(ActivationService.AltLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(ActivationService.BackKeyboardAccelerator);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = navigationView.SettingsItem as NavigationViewItem;
                return;
            }

            Selected = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }

            var item = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
            NavigationService.Navigate(pageType);
        }

        private void HideNavViewBackButton()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                navigationView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            }
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
    }
}
