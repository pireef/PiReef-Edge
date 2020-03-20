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
    class SalinityMeasurementJob : IJob
    {
        DeviceClient _dc;
        private EZOSal _sal;

        public SalinityMeasurementJob(DeviceClient dc, EZOSal sal)
        {
            _dc = dc;
            _sal = sal;
        }

        //salinity circuit

        public void Execute()
        {
            _sal.TakeReading();
            try
            {
                var s = _sal.Response.Substring(1, 5); //I think this works, will need to verify once I get a probe with real values
                                                       //"\u00010.000\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0"
                var f = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
                var task = SendTelemetry(f);
                task.Wait();
            }
            catch (FormatException ex)
            {
                Console.Write("Format Exception ocurred, invalid number recieved: " + _sal.Response + ex.ToString());
            }
            Console.WriteLine("{0}     Salinity Value Is: {1}", DateTime.Now, _sal.Response);
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
