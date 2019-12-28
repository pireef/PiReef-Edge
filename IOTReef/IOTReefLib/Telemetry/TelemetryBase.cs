using System;
using System.Collections.Generic;
using System.Text;

namespace IOTReefLib.Telemetry
{
    public enum TelemetryType
    {
        Doser,
        Sience,
        Power
    };

    public class TelemetryBase
    {
        TelemetryType type;
        DateTime datetime;
        int deviceid;

        public TelemetryBase(TelemetryType type, DateTime dateTime, int deviceid)
        {
            Type = type;
            Datetime = dateTime;
            Deviceid = deviceid;
        }

        public TelemetryType Type { get => type; set => type = value; }
        public DateTime Datetime { get => datetime; set => datetime = value; }
        public int Deviceid { get => deviceid; set => deviceid = value; }
    }
}
