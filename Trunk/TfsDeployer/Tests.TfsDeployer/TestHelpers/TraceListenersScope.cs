using System;
using System.Diagnostics;
using System.Linq;

namespace Tests.TfsDeployer.TestHelpers
{
    class TraceListenersScope : IDisposable
    {
        private TraceListener[] _listeners;

        public TraceListenersScope()
        {
            _listeners = Trace.Listeners.Cast<TraceListener>().ToArray();
        }

        public void Dispose()
        {
            var listeners = _listeners;
            _listeners = null;
            if (listeners == null) return;

            Trace.Listeners.Clear();
            Trace.Listeners.AddRange(listeners);
        }
    }
}