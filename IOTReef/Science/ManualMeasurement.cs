using IOTReefLib.Telemetry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Science
{
    class ManualMeasurement
    {
        ScienceType s_type;
        float value;

        public ScienceType TheType { get => s_type; set => s_type = value; }
        public float Value { get => value; set => this.value = value; }
    }
}
