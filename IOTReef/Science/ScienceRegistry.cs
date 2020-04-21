using FluentScheduler;
using IOTReefLib.Circuits;
using Microsoft.Azure.Devices.Client;
using System.Collections.Generic;

namespace Science
{
    class ScienceRegistry : Registry
    {
        public ScienceRegistry(DeviceClient dc, EZOPH ph, EZOrtd tmp, EZOSal sal, List<float> tmplist, List<float> phlist, List<float> sallist)
        {
            //Delay for 10 seconds, to let everything start up...also stagger the schedule, not that it matters, but it should keep 
            //jobs from running at the exact same time.  
            Schedule(() => new PhMeasurementJob(dc, ph, phlist)).ToRunEvery(2).Seconds().DelayFor(10).Seconds();
            Schedule(() => new TempMeasurementJob(dc, tmp, tmplist)).ToRunEvery(2).Seconds().DelayFor(10).Seconds();
            Schedule(() => new SalinityMeasurementJob(dc, sal, sallist)).ToRunEvery(2).Seconds().DelayFor(10).Seconds();
        }
    }
}
