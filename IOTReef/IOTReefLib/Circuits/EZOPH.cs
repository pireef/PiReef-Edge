using System;
using System.Collections.Generic;
using System.Text;

namespace IOTReefLib.Circuits
{
    public class EZOPH : EZOBase
    {
        public EZOPH(int address) : base(address)
        {
        }

        public EZOPH() : base(0x63)
        {
            //default address of the circuit
            //Address = 0x63;
        }

        public void Find()
        {
            string command = "Find";
            ExecuteCommand(command);
        }

        public void TakeReading()
        {
            string command = "R";
            ExecuteCommand(command, 900);
        }

        public void TakeReadingTempComp(float temp)
        {
            string command = "RT," + temp.ToString();
            ExecuteCommand(command, 900);
        }

        public void Status()
        {
            string command = "Status";
            ExecuteCommand(command, 1500);
        }

        public void CalibrateLow()
        {
            string command = "Cal,low,4.00";
            ExecuteCommand(command);
        }

        public void CalibrateMid()
        {
            string command = "Cal,mid,7.00";
            ExecuteCommand(command);
        }

        public void CalibrateHigh()
        {
            string command = "Cal,high,10.00";
            ExecuteCommand(command);
        }

        public void ClearCalibration()
        {
            string command = "Cal,clear";
            ExecuteCommand(command);
        }

        public void CheckCalibration()
        {
            string command = "Cal,?";
            ExecuteCommand(command);
        }
    }
}
