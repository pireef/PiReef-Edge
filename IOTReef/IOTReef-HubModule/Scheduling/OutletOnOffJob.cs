using FluentScheduler;
using IOTReef_HubModule.Models;

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
