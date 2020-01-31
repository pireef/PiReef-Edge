using System;
using System.Collections.Generic;
using System.Text;
using FluentScheduler;
using Microsoft.Azure.Devices.Client;

namespace Science
{
    class SalinityMeasurementJob : IJob
    {
        DeviceClient _dc;

        public SalinityMeasurementJob(DeviceClient dc)
        {
            _dc = dc;
        }

        //salinity circuit

        public void Execute()
        {
            //simulated data until we get the circuit
            Random rn = new Random();
            var sal = rn.NextDouble() * (35.0 - 34.0) + 34.0;
            Console.WriteLine("{0}     Salinity:{1}", DateTime.Now, sal);
        }
    }
}
