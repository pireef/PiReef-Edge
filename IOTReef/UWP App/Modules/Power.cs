using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_App.Models;
using UWP_App.Scheduling;

namespace UWP_App.Modules
{
    public class Power : ModuleBase
    {
        public Power(string vid, string pid, Dictionary<string, Outlet> dict) : base(vid, pid, dict)
        {
        }

        public override void Device_DeviceReady()
        {
            base.Device_DeviceReady();
            AfterDataConst();
            JobManager.Initialize(new FluentRegistry(OutletDict));
        }

        private void AfterDataConst()
        {
            foreach(var plug in OutletDict)
            {
                plug.Value.AfterDataConst(Device);
                plug.Value.PowerUpRecovery();
            }
        }
    }
}
