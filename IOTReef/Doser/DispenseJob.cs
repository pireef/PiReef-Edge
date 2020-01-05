using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.Azure.Devices.Client;
using IOTReefLib.Telemetry;
using Newtonsoft.Json;

namespace Doser
{
    class DispenseJob : IJob
    {
        private int doser;
        private int numofTimesaDay;
        private string name;
        private DeviceClient dc;

        public DispenseJob(int doser, int numofTimesaDay, string name, DeviceClient dc)
        {
            this.doser = doser;
            this.numofTimesaDay = numofTimesaDay;
            this.name = name;
            this.dc = dc;
        }

        public void Execute()
        {
            
            EZO_pmp doserpmp = new EZO_pmp(doser, name);
            DosingSettings settings = SettingsHelper.ReadSettings();
            float amt;
            
            if (doser == 0x66)
            {
                amt = (float)settings.PMP1Amt / 48;
            }
            else
            {
                amt = (float)settings.PMP2Amt / 48;
            }
            //get the amount to dose per day
            Console.WriteLine("{0}     Executing job on: {1} for {2}ml", DateTime.Now, name, amt);
            using (doserpmp)
            {
                //await doserpmp.Connect();
                doserpmp.Dispense(amt);
                var task = SendTelemetry(doserpmp, amt);
                task.Wait();
                Console.WriteLine("Telemetry sent..");
            }
        }

        private async Task SendTelemetry(EZO_pmp pmp, float amount)
        {
            try
            {
                //need to pack this connection string away
                //string cnString = "HostName=IOT-ReefEdge.azure-devices.net;DeviceId=doser;SharedAccessKey=hd7HPRJSkfd7ioGd8vyMGLK+aql+exBCt/Y/e4Bt1C4=;GatewayHostName=raspberrypi";
                //need to set a variable somewhere to id the device instead of hardcoding that db id
                //var client = DeviceClient.CreateFromConnectionString(cnString);

                //var gid = new Guid("2d3b067e-6411-4b15-b8ba-7254026026f9");
                DoserTelemetry telem = new DoserTelemetry(TelemetryType.Doser, DateTime.Now, "2d3b067e-6411-4b15-b8ba-7254026026f9", amount, pmp.CommonName);
                string strtelem = JsonConvert.SerializeObject(telem);
                var bytes = Encoding.UTF8.GetBytes(strtelem);
                var msg = new Message(bytes);
                await dc.SendEventAsync(msg);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
