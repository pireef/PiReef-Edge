using FluentScheduler;
using System.Collections.Generic;
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
            var fr = new FluentRegistry(OutletDict);
            JobManager.Initialize(fr);
            //JobManager.Initialize(new FluentRegistry(OutletDict));
            PowerUpRecovery(fr);
        }

        private void AfterDataConst()
        {
            foreach (var plug in OutletDict)
            {
                plug.Value.AfterDataConst(Device);
                //plug.Value.PowerUpRecovery();
            }
        }

        private void PowerUpRecovery(FluentRegistry reg)
        {
            foreach (var plug in OutletDict)
            {
                plug.Value.PowerUpRecovery(reg);
            }
        }
    }
}
