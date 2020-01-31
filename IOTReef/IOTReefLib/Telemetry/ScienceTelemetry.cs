using System;
using System.Collections.Generic;
using System.Text;

namespace IOTReefLib.Telemetry
{
    public enum ScienceType
    {
        PH,
        Temperature,
        Salinity,
        Calcium,
        Alkalinity,
        Magnesium,
        Nitrate,
        Phosphate
    }

    public class ScienceTelemetry : TelemetryBase
    {
        ScienceType type;
        float value;

        public ScienceTelemetry(TelemetryType type, DateTime dateTime, string deviceid, float value, ScienceType dataType) : base(type, dateTime, deviceid)
        {
            Value = value;
            DataType = dataType;
        }

        public float Value { get => value; set => this.value = value; }
        internal ScienceType DataType { get => type; set => type = value; }
    }
}
