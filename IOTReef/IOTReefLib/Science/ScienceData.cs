namespace IOTReefLib.Science
{
    public class ScienceData
    {
        //private DateTime timeRead;
        private float temp;
        private float ph;
        private float salinity;
        private float orp;

        public float Temp { get => temp; set => temp = value; }
        public float Ph { get => ph; set => ph = value; }
        public float Salinity { get => salinity; set => salinity = value; }
        public float Orp { get => orp; set => orp = value; }
    }
}
