using System;
using System.Collections.Generic;
using System.Text;
using FluentScheduler;
using IOTReefLib.Circuits;
using Microsoft.Azure.Devices.Client;

namespace Science
{
    class ScienceRegistry : Registry
    {
        public ScienceRegistry(DeviceClient dc, EZOPH ph)
        {
            Schedule(() => new PhMeasurementJob(dc, ph)).ToRunEvery(5).Seconds();
            Schedule(() => new TempMeasurementJob(dc)).ToRunEvery(5).Seconds().DelayFor(1).Seconds();
            Schedule(() => new SalinityMeasurementJob(dc)).ToRunEvery(1).Minutes().DelayFor(2).Seconds();
        }
    }
}
