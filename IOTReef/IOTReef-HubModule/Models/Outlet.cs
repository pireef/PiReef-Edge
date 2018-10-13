using IOTReef_HubModule.Models;
using Microsoft.Maker.RemoteWiring;
using Newtonsoft.Json;
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
        float tempLowTrigger;
        float tempHighTrigger;
        //float phLowTrigger;
        //float phHighTrigger;
        List<Trigger> outletTriggers;

        public Outlet()
        {
            this.OutletTriggers = new List<Trigger>();
        }

        public Outlet(RemoteDevice arduino)
        {
            this.Arduino = arduino;
            this.OutletTriggers = new List<Trigger>();
        }

        public Outlet(byte pinNum, string OutletName, int PlugNumber, OutletState state, OutletState fallback, RemoteDevice arduino)
        {
            this.OutletTriggers = new List<Trigger>();
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
        [JsonIgnore]
        public RemoteDevice Arduino { get => arduino; set => arduino = value; }
        public List<Trigger> OutletTriggers { get => outletTriggers; set => outletTriggers = value; }

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

        public void CheckTriggers(ScienceModuleData data)
        {
            foreach (var trig in outletTriggers)
            {
                //what value are we checking?
                if (trig.DataToCheck == TriggerData.TEMPERATURE)
                {
                    var temp = float.Parse(trig.Value);

                    if(trig.DataOperator == TriggerOperator.GREATERTHAN)
                    {
                        //checking for high temperature
                        if (data.Temp > temp)
                        {
                            //trigger condition exists, now go do the work!
                            TakeTheAction(trig.ActionToTake);                            
                        }
                    }
                    else if(trig.DataOperator == TriggerOperator.LESSTHAN)
                    {
                        //checking for low temperature
                        if(data.Temp < temp)
                        {
                            //trigger condtion exists, now go do the work!
                            TakeTheAction(trig.ActionToTake);
                        }
                    }
                }
                else if(trig.DataToCheck == TriggerData.PH)
                {
                    var ph = float.Parse(trig.Value);
                }
            }
        }

        private void TakeTheAction(Actions actions)
        {
            switch (actions)
            {
                case Actions.OUTLETON:
                    SetState(OutletState.ON);
                    break;
                case Actions.OUTLETOFF:
                    SetState(OutletState.OFF);
                    break;
                case Actions.EMAIL:
                    //not implemented yet
                    break;
                case Actions.TEXTMESSAGE:
                    //not implemented yet
                    break;

            }
        }
    }
}
