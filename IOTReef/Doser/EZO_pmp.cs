using System;
using System.Collections.Generic;
using System.Text;
using System.Device.I2c;
using System.Threading.Tasks;

namespace Doser
{
    class EZO_pmp : IDisposable
    {
        int address;
        I2cDevice dev;
        string commonName;
        string response;

        public EZO_pmp(int PumpAddr)
        {
            address = PumpAddr;
            var cnSettings = new I2cConnectionSettings(1, address);
            dev = I2cDevice.Create(cnSettings);
            //dev.ConnectionSettings = cnSettings;
        }

        public string Response { get => response; }
        public string CommonName { get => commonName; set => commonName = value; }

        public void status()
        {
            string command = "Status";
            ExecuteCommand(command, 400);
        }

        public void Sleep()
        {
            string command = "Sleep";
            ExecuteCommand(command);
        }

        public void Dispense()
        {
            //continuous dispensing
            string command = "D,*";
            ExecuteCommand(command);
        }

        public void Find()
        {
            string command = "find";
            ExecuteCommand(command);
        }

        public double MaxFlow()
        {
            double val = 0;
            string command = "DC,?";

            ExecuteCommand(command, 400);

            if (response.Contains("MAXRATE"))
            {
                // the response was valid, now find the number. 
                // The next response will be the rate, so now convert the number

                string[] initvalues = response.Split(',');
                string[] numvalues = initvalues[1].Split('\0');

                try
                {
                    val = Convert.ToDouble(numvalues[0]);
                }
                catch (FormatException)
                {

                }
                //foreach(string v in numvalues)
                //{
                //    try
                //    {
                //        val = Convert.ToDouble(v);
                //    }
                //    catch (FormatException)
                //    {

                //    }
                //}
            }

            return val;
        }

        public void Dispense(float amount)
        {
            //dose a specific amount
            string command = "D," + amount.ToString();
            ExecuteCommand(command);
        }

        public void Dispense(float amount, float time)
        {
            //dose an amount over time
            string command = "D," + amount.ToString() + "," + time.ToString();
            ExecuteCommand(command);
        }

        public void DispenseConstant(float amount, float time)
        {
            //
            string command = "D," + amount.ToString() + "," + time.ToString();
            ExecuteCommand(command);
        }

        public void Pause()
        {
            string command = "P";
            ExecuteCommand(command);
        }

        public void Stop()
        {
            string command = "X";
            ExecuteCommand(command);
        }

        public void VolumeDispensed()
        {
            string command = "TV,?";
            ExecuteCommand(command, 325);
        }

        public void AbsoluteVolumeDispense()
        {
            string command = "ATV,?";
            ExecuteCommand(command, 325);
        }

        public void Calibration()
        {
            string command = "D,10";
            ExecuteCommand(command);
        }

        public void SetCalibration(string corrVolume)
        {
            string command = "Cal," + corrVolume.ToString();
            ExecuteCommand(command);
        }

        public void ClearCalibration()
        {
            string command = "Cal, Clear";

            ExecuteCommand(command);
        }

        public void CalibrationCheck()
        {
            string command = "Cal,?";
            ExecuteCommand(command, 400);
        }

        private void ExecuteCommand(string command)
        {
            byte[] conv = Encoding.ASCII.GetBytes(command);

            dev.Write(conv);
        }

        private void ExecuteCommand(string command, int delay)
        {
            byte[] readbuf = new byte[20];
            byte[] conv = Encoding.ASCII.GetBytes(command);

            dev.Write(conv);
            Task.Delay(delay);
            dev.Read(readbuf);

            response = Encoding.ASCII.GetString(readbuf, 0, readbuf.Length);

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
