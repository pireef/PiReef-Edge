using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_App.Models
{
    public class OutletVM
    {
        private string _Name;
        private OutletState _currentState;
        private string _IconImg;

        public OutletVM(string name, OutletState currentState)
        {
            Name = name;
            CurrentState = currentState;
        }

        public string Name { get => _Name; set => _Name = value; }
        public OutletState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                if (_currentState == OutletState.ON)
                    _IconImg = "ms-appx:///Assets/Outlet-Icon-On.png";
                else
                    _IconImg = "ms-appx:///Assets/Outlet-Icon-Off.png";
            }
        }
        public string IconImg { get => _IconImg;}
    }
}
