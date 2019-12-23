using System;
using System.Diagnostics;

namespace Doser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Two Part Doser Starting...");
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

#if DEBUG
            while (true)
            {
                Console.WriteLine("Waiting for Debugger to attach...");
                if (Debugger.IsAttached)
                    break;
                System.Threading.Thread.Sleep(1000);
            }
#endif
            //Console.WriteLine("Craeting the device");
            //var pmp = new EZO_pmp(0x67);
            //Console.WriteLine("Sending the Find Command");
            //pmp.Find();
            //System.Threading.Thread.Sleep(5000);
            //pmp.Stop();
            //Console.WriteLine("I should be blinking");
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Two Part Doser Exiting...");
        }
    }
}
