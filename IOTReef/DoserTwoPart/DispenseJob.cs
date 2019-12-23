using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using TwoPartDoser.Helpers;

namespace DoserTwoPart
{
    class DispenseJob : IJob
    {
        private int doser;
        private int numofTimesaDay;

        public DispenseJob(int doser, int numofTimesaDay)
        {
            this.doser = doser;
            this.numofTimesaDay = numofTimesaDay;
        }

        public async void Execute()
        {
            EZO_pmp doserpmp = new EZO_pmp(doser);
            DosingSettings settings = SettingsHelper.ReadSettings();
            float amt;

            if (doser == 0x66)
            {
                amt = settings.AlkalinityAmt / 48;
            }
            else
            {
                amt = settings.CalciumAmt / 48;
            }
            //get the amount to dose per day
            using (doserpmp)
            {
                //await doserpmp.Connect();
                doserpmp.Dispense(amt);
            }
        }
    }
}
