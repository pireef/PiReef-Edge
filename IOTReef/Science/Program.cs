using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IOTReefLib.Circuits;
using Microsoft.Azure.Devices.Client;
using FluentScheduler;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using IOTReefLib.Telemetry;

namespace Science
{
    class Program
    {
        static DeviceClient _deviceclient;
        static void Main(string[] args)
        {
            string cnString = "HostName=IOT-ReefEdge.azure-devices.net;DeviceId=science;SharedAccessKey=xV+L6SUkiwhwH9N7j5Jly85gbeXFPW5dffWIRwGmPyE=;GatewayHostName=raspberrypi";
#if DEBUG
            while (true)
            {
                Console.WriteLine("{0}     Waiting for Debugger to attach...", DateTime.Now);
                if (Debugger.IsAttached)
                    break;
                System.Threading.Thread.Sleep(1000);
            }
#endif

            Console.WriteLine("{0}     Science Module Starting...", DateTime.Now);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            _deviceclient = DeviceClient.CreateFromConnectionString(cnString);
            var ph = new EZOPH();
            //set the direct method handlers here
            _deviceclient.SetMethodHandlerAsync("IntakeManualMeasurement", IntakeManualMeasurement, null).Wait();

            Console.WriteLine("{0}     Installing Certificate...", DateTime.Now);
            InstallCACert();

            Console.WriteLine("{0}     Setting Schedule...", DateTime.Now);

            JobManager.Initialize(new ScienceRegistry(_deviceclient, ph));

            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

        private static async Task<MethodResponse> IntakeManualMeasurement(MethodRequest methodRequest, object userContext)
        {
            var payload = Encoding.UTF8.GetString(methodRequest.Data);
            ManualMeasurement mm = JsonConvert.DeserializeObject<ManualMeasurement>(payload);
            ScienceTelemetry telem = new ScienceTelemetry(TelemetryType.Sience, DateTime.Now, "97765c81-db74-451a-b89d-170ab3464d17", mm.Value, mm.TheType);

            if( telem != null)
            {
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                string s_tem = JsonConvert.SerializeObject(telem);
                var bytes = Encoding.UTF8.GetBytes(s_tem);
                var msg = new Message(bytes);
                await _deviceclient.SendEventAsync(msg);
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                string result = "{\"result\":\"Invalid parameter\"}";
                Console.WriteLine("Did not send data, likely bad data.");
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
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
