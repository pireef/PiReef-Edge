namespace UWP_App.Models
{
    public class Schedule
    {
        private int _hour;
        private int _min;
        private OutletState _newState;

        public Schedule()
        {

        }

        public Schedule(int hour, int min, OutletState newState)
        {
            Hour = hour;
            Min = min;
            NewState = newState;
        }

        public int Hour { get => _hour; set => _hour = value; }
        public int Min { get => _min; set => _min = value; }
        public OutletState NewState { get => _newState; set => _newState = value; }

        public override string ToString()
        {
            string msg;

            msg = "I turn ";

            if (NewState == OutletState.ON)
            {
                msg += "ON at ";
            }
            else if (NewState == OutletState.OFF)
            {
                msg += "OFF at ";
            }

            msg += Hour + ":" + Min + " hours.";

            return msg;
        }
    }
}
