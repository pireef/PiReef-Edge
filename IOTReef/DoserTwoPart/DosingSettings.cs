using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoserTwoPart
{
    class DosingSettings
    {
        int calAmt;
        int alkAmt;
        int calStartTime;
        int alkStartTime;
        int maxRunTime;
        DateTime calLastCalibration;
        DateTime alkLastCalibration;

        public DosingSettings(int alkalinityAmt, int calciumAmt, int calStartTime, int alkStartTime, int maxRunTime)
        {
            AlkalinityAmt = alkalinityAmt;
            CalciumAmt = calciumAmt;
            CalStartTime = calStartTime;
            AlkStartTime = alkStartTime;
            MaxRunTime = maxRunTime;
        }

        public int AlkalinityAmt { get => alkAmt; set => alkAmt = value; }
        public int CalciumAmt { get => calAmt; set => calAmt = value; }
        public int CalStartTime { get => calStartTime; set => calStartTime = value; }
        public int AlkStartTime { get => alkStartTime; set => alkStartTime = value; }
        public int MaxRunTime { get => maxRunTime; set => maxRunTime = value; }
        public DateTime CalLastCalibration { get => calLastCalibration; set => calLastCalibration = value; }
        public DateTime AlkLastCalibration { get => alkLastCalibration; set => alkLastCalibration = value; }
    }
}