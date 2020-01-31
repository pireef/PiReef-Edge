using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FluentScheduler;
using IOTReefLib.Circuits;
using Microsoft.Azure.Devices.Client;

namespace Science
{
    class PhMeasurementJob : IJob
    {
        private DeviceClient _dc;
        private EZOPH ph;

        public PhMeasurementJob(DeviceClient dc, EZOPH ph)
        {
            _dc = dc;
            this.ph = ph;
        }

        public void Execute()
        {
            ph.TakeReading();

            try
            {
                var s = ph.Response.Substring(1, 5); //I think this works, will need to verify once I get a probe with real values
                    //"\u00010.000\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0"
                var f = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (FormatException ex)
            {
                Console.Write("Format Exception ocurred, invalid number recieved: " + ph.Response);
            }
            Console.WriteLine("{0}     PH Value Is: {1}", DateTime.Now, ph.Response);
        }
    }
}
