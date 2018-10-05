using Microsoft.Maker.RemoteWiring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTReef_HubModule
{
    public enum OutletState
    {
        ON,
        OFF
    }

    public class Outlet
    {
        byte pinNum;
        string outletName;
        int plugNumber;
        OutletState state;
        OutletState fallback;
        RemoteDevice arduino;

        public Outlet()
        {
        }

        public Outlet(RemoteDevice arduino)
        {
            this.Arduino = arduino;
        }

        public Outlet(byte pinNum, string OutletName, int PlugNumber, OutletState state, OutletState fallback, RemoteDevice arduino)
        {
            this.pinNum = pinNum;
            this.state = state;
            this.fallback = fallback;
            this.Arduino = arduino;
            this.outletName = OutletName;
            this.plugNumber = PlugNumber;
            arduino.pinMode(pinNum, PinMode.OUTPUT);
            SetState(state);
        }

        public void AfterDataConst(RemoteDevice arduino)
        {
            this.Arduino = arduino;
            this.Arduino.pinMode(pinNum, PinMode.OUTPUT);
            SetState(state);
        }

        public byte PinNum { get => pinNum; set => pinNum = value; }
        public OutletState State { get => state; }
        public OutletState Fallback { get => fallback; set => fallback = value; }
        public string OutletName { get => outletName; set => outletName = value; }
        public int PlugNumber { get => plugNumber; set => plugNumber = value; }
        public RemoteDevice Arduino { get => arduino; set => arduino = value; }

        public void Toggle()
        {
            if (State == OutletState.ON)
            {
                Arduino.digitalWrite(pinNum, PinState.LOW);
                state = OutletState.OFF;
                return;
            }
            if(State == OutletState.OFF)
            {
                Arduino.digitalWrite(pinNum, PinState.HIGH);
                state = OutletState.ON;
                return;
            }
        }

        private void SetState(OutletState newState)
        {
            if (newState == OutletState.ON)
            {
                Arduino.digitalWrite(pinNum, PinState.HIGH);
                state = newState;
                return;
            }
            if (newState == OutletState.OFF)
            {
                Arduino.digitalWrite(pinNum, PinState.LOW);
                state = newState;
                return;
            }
        }
    }
}
