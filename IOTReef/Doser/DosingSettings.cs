using System;

namespace Doser
{
    enum ScheduleType
    {
        Every30Min,
        EveryHour,
        OnceADay,
        TwiceADay,
        OnceAWeek,
        None
    };

    class DosingSettings
    {
        int pmp1Amt;
        int pmp2Amt;
        int maxRunTime;
        DateTime pmp1LastCalibration;
        DateTime pmp2LastCalibration;
        ScheduleType type;
        int hour;
        int min;
        DayOfWeek day;
        bool antiInterference;

        public DosingSettings(int pmp2Amt, int pmp1Amt, int maxRunTime, ScheduleType type, bool bInterference)
        {
            PMP2Amt = pmp2Amt;
            PMP1Amt = pmp1Amt;
            MaxRunTime = maxRunTime;
            Type = type;
            antiInterference = bInterference;
        }

        public int PMP2Amt { get => pmp2Amt; set => pmp2Amt = value; }
        public int PMP1Amt { get => pmp1Amt; set => pmp1Amt = value; }
        public int MaxRunTime { get => maxRunTime; set => maxRunTime = value; }
        public DateTime PMP1LastCalibration { get => pmp1LastCalibration; set => pmp1LastCalibration = value; }
        public DateTime PMP2LastCalibration { get => pmp2LastCalibration; set => pmp2LastCalibration = value; }
        public int Hour { get => hour; set => hour = value; }
        public int Min { get => min; set => min = value; }
        public DayOfWeek Day { get => day; set => day = value; }
        public bool AntiInterference { get => antiInterference; set => antiInterference = value; }
        internal ScheduleType Type { get => type; set => type = value; }
    }
}