using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using IOTReefLib.Telemetry;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

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
            var task = SendTelemetry((float)sal);
            task.Wait();
        }

        private async Task SendTelemetry(float amt)
        {
            try
            {
                ScienceTelemetry telem = new ScienceTelemetry(TelemetryType.Sience, DateTime.Now, "97765c81-db74-451a-b89d-170ab3464d17", amt, ScienceType.Salinity);
                string str = JsonConvert.SerializeObject(telem);
                var bytes = Encoding.UTF8.GetBytes(str);
                var msg = new Message(bytes);
                await _dc.SendEventAsync(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
