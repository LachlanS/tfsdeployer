using System.Diagnostics;

namespace TfsDeployer
{
    public class TraceSwitchLog : ILog
    {
        private readonly TraceSwitch _traceSwitch;

        public TraceSwitchLog(TraceSwitch traceSwitch)
        {
            _traceSwitch = traceSwitch;
        }

        public void Information(string message)
        {
            if (!_traceSwitch.TraceInfo) return;
            Trace.TraceInformation(message);
        }

        public void Warning(string message)
        {
            if (!_traceSwitch.TraceWarning) return;
            Trace.TraceWarning(message);
        }

        public void Error(string message)
        {
            if (!_traceSwitch.TraceError) return;
            Trace.TraceError(message);
        }
    }
}