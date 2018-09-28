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
        OutletState state;
        OutletState fallback;
        RemoteDevice arduino;

        public Outlet(RemoteDevice arduino)
        {
            this.arduino = arduino;
        }

        public Outlet(byte pinNum, OutletState state, OutletState fallback, RemoteDevice arduino)
        {
            this.pinNum = pinNum;
            this.state = state;
            this.fallback = fallback;
            this.arduino = arduino;
            arduino.pinMode(pinNum, PinMode.OUTPUT);
            SetState(state);
        }

        public byte PinNum { get => pinNum; set => pinNum = value; }
        public OutletState State { get => state; }
        public OutletState Fallback { get => fallback; set => fallback = value; }

        public void Toggle()
        {
            if (State == OutletState.ON)
            {
                arduino.digitalWrite(pinNum, PinState.LOW);
                state = OutletState.OFF;
                return;
            }
            if(State == OutletState.OFF)
            {
                arduino.digitalWrite(pinNum, PinState.HIGH);
                state = OutletState.ON;
                return;
            }
        }

        private void SetState(OutletState newState)
        {
            if (newState == OutletState.ON)
            {
                arduino.digitalWrite(pinNum, PinState.HIGH);
                state = newState;
                return;
            }
            if (newState == OutletState.OFF)
            {
                arduino.digitalWrite(pinNum, PinState.LOW);
                state = newState;
                return;
            }
        }
    }
}
