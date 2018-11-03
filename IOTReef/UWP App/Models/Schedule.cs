using UWP_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_App.Models
{
    public class Schedule
    {
        private int _hour;
        private int _min;
        private OutletState _newState;

        public Schedule(int hour, int min, OutletState newState)
        {
            Hour = hour;
            Min = min;
            NewState = newState;
        }

        public int Hour { get => _hour; set => _hour = value; }
        public int Min { get => _min; set => _min = value; }
        public OutletState NewState { get => _newState; set => _newState = value; }
    }
}
