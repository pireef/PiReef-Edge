using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;

namespace Doser
{
    class DoserRegistry : Registry
    {
        public DoserRegistry()
        {
            Schedule(() => new DispenseJob(0x67, 48, "Pump 1")).ToRunEvery(30).Minutes();
            Schedule(() => new DispenseJob(0x66, 48, "Pump 2")).ToRunEvery(30).Minutes().DelayFor(15).Minutes();
        }
    }
}