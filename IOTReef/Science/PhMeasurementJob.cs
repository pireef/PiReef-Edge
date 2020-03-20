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
                var task = SendTelemetry(f);
                task.Wait();
            }
            catch (FormatException ex)
            {
                Console.Write("Format Exception ocurred, invalid number recieved: " + ph.Response + ex.ToString());
            }
            Console.WriteLine("{0}     PH Value Is: {1}", DateTime.Now, ph.Response);
        }

        private async Task SendTelemetry(float amt)
        {
            try
            {
                ScienceTelemetry sTem = new ScienceTelemetry(TelemetryType.Sience, DateTime.Now, "97765c81-db74-451a-b89d-170ab3464d17", amt, ScienceType.PH);
                string telem = JsonConvert.SerializeObject(sTem);
                var bytes = Encoding.UTF8.GetBytes(telem);
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
