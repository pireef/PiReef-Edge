using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using FluentScheduler;

namespace Doser
{
    class Program
    {
        static void Main(string[] args)
        {
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
            Console.WriteLine("{0}     Installing Certificate...", DateTime.Now);
            InstallCACert();
            Console.WriteLine("{0}     Setting up schedule...", DateTime.Now);
            JobManager.Initialize(new DoserRegistry());
            Console.WriteLine("{0}     Scheduler Set", DateTime.Now);

            while (true)
            {
                //just a loop to let the scheduler do it's thing
                System.Threading.Thread.Sleep(10000);
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
