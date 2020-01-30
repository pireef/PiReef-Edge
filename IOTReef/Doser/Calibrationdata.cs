using System;
using System.Collections.Generic;
using System.Text;

namespace Doser
{
    class Calibrationdata
    {
        int pump;
        float val;

        public float Val { get => val; set => val = value; }
        public int Pump { get => pump; set => pump = value; }
    }
}
