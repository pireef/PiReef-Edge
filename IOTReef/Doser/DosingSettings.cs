using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doser
{
    class DosingSettings
    {
        float pmp1Amt;
        float pmp2Amt;
        int maxRunTime;
        DateTime pmp1LastCalibration;
        DateTime pmp2LastCalibration;

        public DosingSettings(int pmp2Amt, int pmp1Amt, int maxRunTime)
        {
            PMP2Amt = pmp2Amt;
            PMP1Amt = pmp1Amt;
            MaxRunTime = maxRunTime;
        }

        public float PMP2Amt { get => pmp2Amt; set => pmp2Amt = value; }
        public float PMP1Amt { get => pmp1Amt; set => pmp1Amt = value; }
        public int MaxRunTime { get => maxRunTime; set => maxRunTime = value; }
        public DateTime PMP1LastCalibration { get => pmp1LastCalibration; set => pmp1LastCalibration = value; }
        public DateTime PMP2LastCalibration { get => pmp2LastCalibration; set => pmp2LastCalibration = value; }
    }
}