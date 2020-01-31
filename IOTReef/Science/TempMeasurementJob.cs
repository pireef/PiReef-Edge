using System;
using System.Collections.Generic;
using System.Text;
using FluentScheduler;
using Microsoft.Azure.Devices.Client;

namespace Science
{
    class TempMeasurementJob : IJob
    {
        private DeviceClient _dc;
        //Temp Circuit

        public TempMeasurementJob(DeviceClient dc)
        {
            _dc = dc;
        }



        public void Execute()
        {
            //just simulating data right now until we get the circuits
            Random rn = new Random();
            var temp = rn.NextDouble() * (79.0 - 77.5) + 77.5;
            Console.WriteLine("{0}     Temperature:{1}", DateTime.Now, temp);
        }
    }
}
