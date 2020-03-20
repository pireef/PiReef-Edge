using System;
using System.Globalization;

namespace IOTReefLib.Circuits
{
    public class EZOSal : EZOBase
    {
        public EZOSal(int address) : base(address)
        {

        }

        public EZOSal() : base(0x64)
        {
            //default address of the circuit
        }

        public void TakeReading()
        {
            string command = "R";
            ExecuteCommand(command, 700);
        }

        public void TakeReadingTempComp(float temp)
        {
            string command = "RT," + temp.ToString(CultureInfo.InvariantCulture);
            ExecuteCommand(command, 700);
        }

        public void SetProbeType(string type)
        {
            string command = "K," + type;
            ExecuteCommand(command, 300);
        }

        public void OutputEC(bool set)
        {
            int n = Convert.ToInt32(set);
            string command = "O,EC," + n.ToString(CultureInfo.InvariantCulture);
            ExecuteCommand(command, 300);
        }

        public void OutputSal(bool set)
        {
            int n = Convert.ToInt32(set);
            string command = "O,S," + n.ToString(CultureInfo.InvariantCulture);
            ExecuteCommand(command, 300);
        }

        public void OutputTDS(bool set)
        {
            int n = Convert.ToInt32(set);
            string command = "O,TDS," + n.ToString(CultureInfo.InvariantCulture);
            ExecuteCommand(command, 300);
        }

        public void OutputSG(bool set)
        {
            int n = Convert.ToInt32(set);
            string command = "O,SG," + n.ToString(CultureInfo.InvariantCulture);
            ExecuteCommand(command, 300);
        }

        public void CalibrateDry()
        {
            string command = "Cal,Dry";
            ExecuteCommand(command, 600);
        }

        public void CalibrateLow()
        {
            string command = "Cal,low,12880";
            ExecuteCommand(command, 600);
        }

        public void CalibrateHigh()
        {
            string command = "Cal,high,80000";
            ExecuteCommand(command, 800);
        }

        public void CalibrateClear()
        {
            ExecuteCommand("Cal,Clear", 800);
        }

        public void FactoryReset()
        {
            ExecuteCommand("Factory");
        }
    }
}
