using System;
using System.Collections.Generic;
using System.Text;
using System.Device.I2c;
using System.Threading.Tasks;
using System.IO;

namespace IOTReefLib.Circuits
{
    public class EZOBase : IDisposable
    {
        int address;
        I2cDevice device;
        string response;

        public int Address { get => address; set => address = value; }
        public string Response { get => response; set => response = value; }

        public EZOBase(int address)
        {
            this.Address = address;
            var cnSettings = new I2cConnectionSettings(1, address);
            device = I2cDevice.Create(cnSettings);
        }

        public EZOBase()
        {
            var cnSettings = new I2cConnectionSettings(1, Address);
            device = I2cDevice.Create(cnSettings);
        }

        public void Dispose()
        {
            device.Dispose();
            //GC.SuppressFinalize(this);
        }

        protected void ExecuteCommand(string cmd)
        {
            byte[] conv = Encoding.ASCII.GetBytes(cmd);

            device.Write(conv);
        }

        protected void ExecuteCommand(string cmd, int delay)
        {
            byte[] readbuf = new byte[45];
            byte[] conv = Encoding.ASCII.GetBytes(cmd);

            //device.WriteRead(conv, readbuf);
            device.Write(conv);
            System.Threading.Thread.Sleep(delay);
            try
            {
                device.Read(readbuf);
            }
            catch(IOException ex)
            {
                Console.WriteLine("Got Exception");
                //Console.WriteLine(ex.ToString());
                //System.Threading.Thread.Sleep(delay);
                //device.Read(readbuf);
            }

            Response = Encoding.ASCII.GetString(readbuf, 0, readbuf.Length);
        }
    }
}
