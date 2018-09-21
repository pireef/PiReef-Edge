using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTReef_HubModule.Models
{
    public class ScienceModuleData
    {
        private DateTime timeRead;
        private bool readSuccess;
        private float temp;
        private float ph;
        private float salinity;
        private float dissolvedO2;

      
        public float Temp { get => temp; set => temp = value; }
        public bool ReadSuccess { get => readSuccess; set => readSuccess = value; }
        public DateTime TimeRead { get => timeRead; set => timeRead = value; }
        public float PH { get => ph; set => ph = value; }
        public float Salinity { get => salinity; set => salinity = value; }
        public float DissolvedO2 { get => dissolvedO2; set => dissolvedO2 = value; }
    }
}
