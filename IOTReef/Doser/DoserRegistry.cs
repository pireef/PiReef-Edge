using FluentScheduler;
using Microsoft.Azure.Devices.Client;

namespace Doser
{
    class DoserRegistry : Registry
    {
        public DoserRegistry(DeviceClient dc)
        {
            Schedule(() => new DispenseJob(0x67, 48, "Pump 1", dc)).ToRunEvery(30).Minutes();
            Schedule(() => new DispenseJob(0x66, 48, "Pump 2", dc)).ToRunEvery(30).Minutes().DelayFor(15).Minutes();
        }
    }
}