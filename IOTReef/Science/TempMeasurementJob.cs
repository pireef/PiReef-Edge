using System;
using System.Collections.Generic;
using System.Text;
using FluentScheduler;
using Microsoft.Azure.Devices.Client;
using IOTReefLib.Telemetry;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
            var task = SendTelemetry((float)temp);
            task.Wait();
        }

        private async Task SendTelemetry(float amt)
        {
            try
            {
                ScienceTelemetry telem = new ScienceTelemetry(TelemetryType.Sience, DateTime.Now, "97765c81-db74-451a-b89d-170ab3464d17", amt, ScienceType.Temperature);
                string str = JsonConvert.SerializeObject(telem);
                var bytes = Encoding.UTF8.GetBytes(str);
                var msg = new Message(bytes);
                await _dc.SendEventAsync(msg);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
