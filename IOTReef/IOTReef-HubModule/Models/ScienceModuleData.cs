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

      
        public float Temp { get => temp; set => temp = value; }
        public bool ReadSuccess { get => readSuccess; set => readSuccess = value; }
        public DateTime TimeRead { get => timeRead; set => timeRead = value; }
        public float PH { get => ph; set => ph = value; }
    }
}
