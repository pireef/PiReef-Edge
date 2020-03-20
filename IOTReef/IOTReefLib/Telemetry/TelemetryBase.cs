using System;

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
        string deviceid;

        public TelemetryBase(TelemetryType type, DateTime dateTime, string deviceid)
        {
            Type = type;
            Datetime = dateTime;
            Deviceid = deviceid;
        }

        public TelemetryType Type { get => type; set => type = value; }
        public DateTime Datetime { get => datetime; set => datetime = value; }
        public string Deviceid { get => deviceid; set => deviceid = value; }
    }
}
