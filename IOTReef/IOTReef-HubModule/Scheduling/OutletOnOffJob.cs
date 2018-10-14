using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using IOTReef_HubModule.Models;
using Microsoft.Maker.RemoteWiring;

namespace IOTReef_HubModule.Scheduling
{
    public class OutletOnOffJob : IJob
    {
        private Outlet _outlet;
        private OutletState _newState;

        public OutletOnOffJob(Outlet outlet, OutletState state)
        {
            _outlet = outlet;
            _newState = state;
        }

        public void Execute()
        {
            _outlet.SetState(_newState);
        }
    }
}
