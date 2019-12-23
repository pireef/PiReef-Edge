using System;
using IOTReefLib.Science;

namespace TelemetryGrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IOT Reef Telemetry Grabber starting...");

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("IOT Reef Telemetry Grabber exiting.");
        }
    }
}
