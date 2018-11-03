using UWP_App.Models;
using Microsoft.Maker.RemoteWiring;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_App.Models
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
        List<Trigger> outletTriggers;
        List<Schedule> outletSchedules;

        public Outlet()
        {
            this.OutletTriggers = new List<Trigger>();
            this.OutletSchedules = new List<Schedule>();
        }

        public Outlet(RemoteDevice arduino)
        {
            this.Arduino = arduino;
            this.OutletTriggers = new List<Trigger>();
            this.OutletSchedules = new List<Schedule>();
        }

        public Outlet(byte pinNum, string OutletName, int PlugNumber, OutletState state, OutletState fallback, RemoteDevice arduino)
        {
            this.OutletTriggers = new List<Trigger>();
            this.OutletSchedules = new List<Schedule>();
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
        public List<Schedule> OutletSchedules { get => outletSchedules; set => outletSchedules = value; }

        public void Toggle()
        {
            if (State == OutletState.ON)
            {
                Arduino.digitalWrite(pinNum, PinState.HIGH);
                state = OutletState.OFF;
                return;
            }
            if(State == OutletState.OFF)
            {
                Arduino.digitalWrite(pinNum, PinState.LOW);
                state = OutletState.ON;
                return;
            }
        }

        public void SetState(OutletState newState)
        {
            if (newState == OutletState.ON)
            {
                Arduino.digitalWrite(pinNum, PinState.LOW);
                state = newState;
                return;
            }
            if (newState == OutletState.OFF)
            {
                Arduino.digitalWrite(pinNum, PinState.HIGH);
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

        public void PowerUpRecovery()
        {
            TimeSpan start;
            TimeSpan end;

            foreach(var sched in OutletSchedules)
            {
                if(sched.NewState == OutletState.ON)
                {
                    start = new TimeSpan(sched.Hour, sched.Min, 0);   
                }
                else if(sched.NewState == OutletState.OFF)
                {
                    end = new TimeSpan(sched.Hour, sched.Min, 0);
                }
            }

            if (start != new TimeSpan() && end != new TimeSpan())
            {
                if (CheckTime(DateTime.Now, start, end))
                {
                    //the current time is during the time the outlet should be on, so turn it on
                    SetState(OutletState.ON);
                }
                else
                {
                    //the current time is during the time the outlet should be off, so turn it off
                    SetState(OutletState.OFF);
                }
            }
        }

        private bool CheckTime(DateTime now, TimeSpan start, TimeSpan end)
        {
            TimeSpan current = now.TimeOfDay;

            if (start < end)
                return start <= current && current <= end;

            return !(end < current && current < start);
        }
    }
}
