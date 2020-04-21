using System;

namespace IOTReef_HubModule.Models
{
    public class ScienceModuleData
    {
        private DateTime timeRead;
        private bool readSuccess;
        private float temp;
        private float ph;
        private float salinity;
        private float orp;
        private bool converttoF;


        public float Temp { get => temp; set => temp = (value * 9) / 5 + 32; }
        public bool ReadSuccess { get => readSuccess; set => readSuccess = value; }
        public DateTime TimeRead { get => timeRead; set => timeRead = value; }
        public float PH { get => ph; set => ph = value; }
        public float Salinity { get => salinity; set => salinity = value; }
        public float ORP { get => orp; set => orp = value; }
        public bool ConverttoF { get => converttoF; set => converttoF = value; }
    }
}
