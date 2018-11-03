using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using UWP_App.Models;

namespace UWP_App.Scheduling
{
    public class FluentRegistry : Registry
    {
        public FluentRegistry(Dictionary<string, Outlet> outlets)
        {
            foreach(var plug in outlets)
            {
                foreach(var sched in plug.Value.OutletSchedules)
                {
                    Schedule(() => new OutletOnOffJob(plug.Value, sched.NewState)).ToRunEvery(1).Days().At(sched.Hour, sched.Min);
                }
            }
            //Schedule(() => new OutletOnOffJob(outlet, OutletState.ON)).ToRunEvery(1).Days().At(12, 33);
            //Schedule(() => new OutletOnOffJob(outlet, OutletState.OFF)).ToRunEvery(1).Days().At(12, 34);
        }
    }
}
