using FluentScheduler;
using IOTReefLib.Circuits;
using IOTReefLib.Telemetry;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Science
{
    class TempMeasurementJob : IJob
    {
        private DeviceClient _dc;
        private EZOrtd tmp;

        public TempMeasurementJob(DeviceClient dc, EZOrtd tmp)
        {
            _dc = dc;
            this.tmp = tmp;
        }

        public void Execute()
        {
            tmp.TakeReading();

            var s = tmp.Response.Substring(1, 6);
            var f = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
            Console.WriteLine("{0}     Temperature:{1}", DateTime.Now, f);
            var task = SendTelemetry(f);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
