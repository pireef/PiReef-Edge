using FluentScheduler;
using IOTReefLib.Circuits;
using IOTReefLib.Telemetry;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Science
{
    class TempMeasurementJob : IJob
    {
        private DeviceClient _dc;
        private EZOrtd tmp;
        private List<float> list;

        public TempMeasurementJob(DeviceClient dc, EZOrtd tmp, List<float> tmplist)
        {
            _dc = dc;
            this.tmp = tmp;
            list = tmplist;
        }

        public void Execute()
        {
            tmp.TakeReading();

            var s = tmp.Response.Substring(1, 6);
            var f = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
            Console.WriteLine("{0}     Temperature:{1}", DateTime.Now, f);
            list.Add(f);
            if (list.Count >= 5)
            {
                var task = SendTelemetry(list.Average());
                list.Clear();
                task.Wait();
            }
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
