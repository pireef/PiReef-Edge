using System;
using System.Collections.Generic;
using System.Text;

namespace IOTReefLib.Telemetry
{
    public class DoserTelemetry : TelemetryBase
    {
        float amtdosed;
        string name;

        public DoserTelemetry(TelemetryType type, DateTime dateTime, string deviceid, float amtdosed, string name) : base(type, dateTime, deviceid)
        {
            Amtdosed = amtdosed;
            Name = name;
        }

        public float Amtdosed { get => amtdosed; set => amtdosed = value; }
        public string Name { get => name; set => name = value; }
    }
}
