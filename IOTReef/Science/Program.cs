using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IOTReefLib.Circuits;
using Microsoft.Azure.Devices.Client;
using FluentScheduler;

namespace Science
{
    class Program
    {
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

            var _deviceclient = DeviceClient.CreateFromConnectionString(cnString);
            var ph = new EZOPH();
            //set the direct method handlers here

            Console.WriteLine("{0}     Installing Certificate...", DateTime.Now);
            InstallCACert();

            Console.WriteLine("{0}     Setting Schedule...", DateTime.Now);

            JobManager.Initialize(new ScienceRegistry(_deviceclient, ph));

            while(true)
            {
                System.Threading.Thread.Sleep(10000);
            }

            //EZOPH ph = new EZOPH();
            //ph.Find();
            //System.Threading.Thread.Sleep(1000);
            //ph.Status();
            //System.Threading.Thread.Sleep(1000);
            //Console.WriteLine(ph.Response);
            //System.Threading.Thread.Sleep(1000);
            //Console.WriteLine("Taking 1000 readings");

            //for (int i = 0; i < 1000; i++)
            //{
            //    ph.TakeReading();
            //    System.Threading.Thread.Sleep(100);
            //    Console.WriteLine(ph.Response + "     {0}", DateTime.Now);
            //}
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
