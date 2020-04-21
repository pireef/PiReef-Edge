using FluentScheduler;
using System;
using System.Collections.Generic;
using UWP_App.Models;

namespace UWP_App.Scheduling
{
    public class FluentRegistry : Registry
    {
        public FluentRegistry(Dictionary<string, Outlet> outlets)
        {
            foreach (var plug in outlets)
            {
                foreach (var sched in plug.Value.OutletSchedules)
                {
                    Schedule(() => new OutletOnOffJob(plug.Value, sched.NewState)).ToRunEvery(1).Days().At(sched.Hour, sched.Min);
                }
            }
        }

        public void AddOneTimeJob(Outlet outlet, OutletState newState, DateTime tm)
        {
            Schedule(() => new OutletOnOffJob(outlet, newState)).ToRunOnceAt(tm);
        }
    }
}
