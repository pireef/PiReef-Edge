using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_App.Helpers;
using UWP_App.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OutletInformation : Page
    {
        private Outlet selectedOutlet;
        private bool bChanges = false;

        public OutletInformation()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                string param = (string)e.Parameter;
                if(ShellPage.outletDict.ContainsKey(param))
                {
                    selectedOutlet = ShellPage.outletDict[param];
                }
            }

            if (selectedOutlet.State == OutletState.ON)
                currentState.IsOn = true;
            else
                currentState.IsOn = false;
            if (selectedOutlet.Fallback == OutletState.ON)
                fallbackState.IsOn = true;
            else
                fallbackState.IsOn = false;

            currentState.Toggled += currentState_Toggled;
            GetTriggers();
            GetSchedules();
            base.OnNavigatedTo(e);
        }

        private void GetTriggers()
        {
            foreach(var trig in selectedOutlet.OutletTriggers)
            {
                triggerListView.Items.Add(trig.ToString());
            }
        }

        private void GetSchedules()
        {
            foreach (var sch in selectedOutlet.OutletSchedules)
            {
                scheduleListView.Items.Add(sch.ToString());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            currentState.Toggled -= currentState_Toggled;
            if(bChanges)
            {
                OutletStorage.SaveOutletDictionaryAsync(ShellPage.outletDict, "dictionarysettings.txt");
            }
            base.OnNavigatedFrom(e);
        }
        private void currentState_Toggled(object sender, RoutedEventArgs e)
        {
            selectedOutlet.Toggle();
        }

        private void fallbackState_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddTrigger_Click(object sender, RoutedEventArgs e)
        {
            bChanges = true;

            Trigger newTrigger = new Trigger();

            if(cboData.SelectedIndex == 0)
            {
                newTrigger.DataToCheck = TriggerData.TEMPERATURE;
            }
            else if (cboData.SelectedIndex == 1)
            {
                newTrigger.DataToCheck = TriggerData.PH;
            }

            if(cboOperator.SelectedIndex == 0)
            {
                newTrigger.DataOperator = TriggerOperator.GREATERTHAN;
            }
            else if (cboOperator.SelectedIndex == 1)
            {
                newTrigger.DataOperator = TriggerOperator.LESSTHAN;
            }

            newTrigger.Value = txtValue.Text;

            if(cboAction.SelectedIndex == 0)
            {
                newTrigger.ActionToTake = Actions.OUTLETON;
            }
            else if(cboAction.SelectedIndex == 1)
            {
                newTrigger.ActionToTake = Actions.OUTLETOFF;
            }

            selectedOutlet.OutletTriggers.Add(newTrigger);
            ClearControls();

        }

        private void btnClearTrigger_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            cboData.SelectedIndex = -1;
            cboOperator.SelectedIndex = -1;
            txtValue.Text = "";
            cboAction.SelectedIndex = -1;
            cboSchAction.SelectedIndex = -1;
            txtHour.Text = "";
            txtMin.Text = "";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewSchedule_Click(object sender, RoutedEventArgs e)
        {
            Schedule newSched = new Schedule();
            int hour;
            int min;

            if (Int32.TryParse(txtHour.Text, out hour) & (Int32.TryParse(txtMin.Text, out min)))
            {
                newSched.Hour = hour;
                newSched.Min = min;
            }
            else
            {//not an integer
                return;
            }

            if (cboSchAction.SelectedIndex == 0)
            {
                newSched.NewState = OutletState.ON;
            }
            else if(cboSchAction.SelectedIndex == 1)
            {
                newSched.NewState = OutletState.OFF;
            }

            selectedOutlet.OutletSchedules.Add(newSched);
            bChanges = true;
            ClearControls();
            
        }

        private void btnClearSchedule_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
    }
}
