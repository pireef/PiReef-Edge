using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Doser
{
    class Program
    {
        static void Main(string[] args)
        {
            string cnString = "HostName=IOT-ReefEdge.azure-devices.net;DeviceId=doser;SharedAccessKey=hd7HPRJSkfd7ioGd8vyMGLK+aql+exBCt/Y/e4Bt1C4=;GatewayHostName=raspberrypi";
            Console.WriteLine("{0}     Two Part Doser Starting...", DateTime.Now);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

#if DEBUG
            while (true)
            {
                Console.WriteLine("{0}     Waiting for Debugger to attach...", DateTime.Now);
                if (Debugger.IsAttached)
                    break;
                System.Threading.Thread.Sleep(1000);
            }
#endif
            //should modify the registry to pass a refernce to the device client
            var _deviceclient = DeviceClient.CreateFromConnectionString(cnString);
            _deviceclient.SetMethodHandlerAsync("SetDoserSettings", SetDoserSettings, null).Wait();
            _deviceclient.SetMethodHandlerAsync("CalibrationDispense", CalibrationDispense, null).Wait();
            _deviceclient.SetMethodHandlerAsync("SetCalibration", SetCalibration, null).Wait();

            Console.WriteLine("{0}     Installing Certificate...", DateTime.Now);
            InstallCACert();
            Console.WriteLine("{0}     Setting up schedule...", DateTime.Now);
            JobManager.Initialize(new DoserRegistry(_deviceclient));
            Console.WriteLine("{0}     Scheduler Set", DateTime.Now);

            while (true)
            {
                //just a loop to let the scheduler do it's thing
                System.Threading.Thread.Sleep(10000);
            }
        }

        private static Task<MethodResponse> SetCalibration(MethodRequest methodRequest, object userContext)
        {
            var payload = Encoding.UTF8.GetString(methodRequest.Data);
            Calibrationdata cd = JsonConvert.DeserializeObject<Calibrationdata>(payload);
            Console.WriteLine("{0}     Setting calibration on pump {1}", DateTime.Now, cd.Pump);
            EZO_pmp pmp;

            DosingSettings ds = SettingsHelper.ReadSettings();

            switch(cd.Pump)
            {
                case 1:
                    pmp = new EZO_pmp(0x67, "Pump 1");
                    pmp.SetCalibration(cd.Val.ToString());
                    ds.PMP1LastCalibration = DateTime.Now;
                    break;
                case 2:
                    pmp = new EZO_pmp(0x66, "Pump 2");
                    pmp.SetCalibration(cd.Val.ToString());
                    ds.PMP2LastCalibration = DateTime.Now;
                    break;
            }
            SettingsHelper.WriteSettings(ds);
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));

        }

        private static Task<MethodResponse> CalibrationDispense(MethodRequest methodRequest, object userContext)
        {
            var payload = Encoding.UTF8.GetString(methodRequest.Data);
            EZO_pmp pmp;
            Console.WriteLine("{0}     Begin calibration sequence.  Dispensing 10ml on {1}", DateTime.Now, payload);

            if(Int32.TryParse(payload, out int pumpnum))
            {
                if (pumpnum == 1 | pumpnum == 2)
                {
                    switch(pumpnum)
                    {
                        case 1:
                            pmp = new EZO_pmp(0x67, "Pump 1");
                            pmp.Calibration();
                            break;
                        case 2:
                            pmp = new EZO_pmp(0x66, "Pump 2");
                            pmp.Calibration();
                            break;
                    }
                    string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
                }
                else
                {
                    string result = "{\"result\":\"Invalid Pump Number " + methodRequest.Name + "\"}";
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
                }
            }
            else
            {
                string result = "{\"result\":\"Not a Valid Number given " + methodRequest.Name + "\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("Not a Valid Number"), 400));
            }
        }

        private static Task<MethodResponse> SetDoserSettings(MethodRequest methodRequest, object userContext)
        {
            var payload = Encoding.UTF8.GetString(methodRequest.Data);
            DosingSettings ds = JsonConvert.DeserializeObject<DosingSettings>(payload);

            if(ds != null)
            {
                SettingsHelper.WriteSettings(ds);
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                Console.WriteLine("Saved new settings.");
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                string result = "{\"result\":\"Invalid parameter\"}";
                Console.WriteLine("Did not save settings, likely bad file.");
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
            
        }

        static void InstallCACert()
        {
            string trustedCACertPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            trustedCACertPath += "/azure-iotreef.root.ca.cert.pem";
            
            if (!string.IsNullOrWhiteSpace(trustedCACertPath))
            {
                Console.WriteLine("User configured CA certificate path: {0}", trustedCACertPath);
                if (!File.Exists(trustedCACertPath))
                {
                    // cannot proceed further without a proper cert file
                    Console.WriteLine("Certificate file not found: {0}", trustedCACertPath);
                    throw new InvalidOperationException("Invalid certificate file.");
                }
                else
                {
                    Console.WriteLine("Attempting to install CA certificate: {0}", trustedCACertPath);
                    X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(new X509Certificate2(X509Certificate.CreateFromCertFile(trustedCACertPath)));
                    Console.WriteLine("Successfully added certificate: {0}", trustedCACertPath);
                    store.Close();
                }
            }
            else
            {
                Console.WriteLine("CA_CERTIFICATE_PATH was not set or null, not installing any CA certificate");
            }
        }
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("{0}     Two Part Doser Exiting...", DateTime.Now);
        }
    }
}
