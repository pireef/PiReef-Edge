using FluentScheduler;
using IOTReefLib.Circuits;
using Microsoft.Azure.Devices.Client;

namespace Science
{
    class ScienceRegistry : Registry
    {
        public ScienceRegistry(DeviceClient dc, EZOPH ph, EZOrtd tmp, EZOSal sal)
        {
            //Delay for 10 seconds, to let everything start up...also stagger the schedule, not that it matters, but it should keep 
            //jobs from running at the exact same time.  
            Schedule(() => new PhMeasurementJob(dc, ph)).ToRunEvery(5).Seconds().DelayFor(10).Seconds();
            Schedule(() => new TempMeasurementJob(dc, tmp)).ToRunEvery(5).Seconds().DelayFor(12).Seconds();
            Schedule(() => new SalinityMeasurementJob(dc, sal)).ToRunEvery(1).Minutes();
        }
    }
}
