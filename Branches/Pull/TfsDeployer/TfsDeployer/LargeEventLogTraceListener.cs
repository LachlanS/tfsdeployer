using System;
using System.Diagnostics;

namespace TfsDeployer
{
    class LargeEventLogTraceListener : TraceListener
    {
        private const int MaximumMessageLength = 0x7000;
        private readonly EventLogTraceListener _listener;

        public LargeEventLogTraceListener(string source)
        {
            _listener = new EventLogTraceListener(source);
        }

        public override void Close()
        {
            _listener.Close();
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            _listener.Dispose();
            base.Dispose(disposing);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            foreach (var splitMessage in SplitMessage(message))
            {
                _listener.TraceEvent(eventCache, source, eventType, id, splitMessage);
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
        }

        public override void Write(string message)
        {
            foreach (var splitMessage in SplitMessage(message))
            {
                _listener.Write(splitMessage);
            }
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        private string[] SplitMessage(string message)
        {
            if (message.Length <= MaximumMessageLength)
            {
                return new[] {message};
            }
            var split = new string[(int)Math.Ceiling((double)message.Length / MaximumMessageLength)];
            for (int index = 0; index < split.Length; index++)
            {
                var offset = index * MaximumMessageLength;
                split[index] = message.Substring(offset, Math.Min(message.Length - offset, MaximumMessageLength));
            }
            return split;
        }
    }
}