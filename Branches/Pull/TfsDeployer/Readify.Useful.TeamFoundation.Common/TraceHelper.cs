using System;
using System.Diagnostics;

namespace Readify.Useful.TeamFoundation.Common
{

    public static class TraceHelper
    {

        public static void TraceError(TraceSwitch traceSwitch, string information)
        {
            ValidateArgumentIsNull(traceSwitch);
            if (traceSwitch.TraceError)
            {
                Trace.TraceError(information);
            }
        }

        
        public static void TraceError(TraceSwitch traceSwitch, Exception exception)
        {
            ValidateArgumentIsNull(traceSwitch);
            ValidateArgumentIsNull(exception);
            if (traceSwitch.TraceError)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        public static void TraceError(TraceSwitch traceSwitch, string format, params object[] args)
        {
            ValidateArgumentIsNull(traceSwitch);
            ValidateArgumentIsNull(args);
            if (traceSwitch.TraceError)
            {
                if (args != null && args.Length > 0)
                {
                    Trace.TraceError(format, args);
                }
                else
                {
                    Trace.TraceError(format);
                }
            }
        }


        public static void TraceInformation(TraceSwitch traceSwitch, string information)
        {
            ValidateArgumentIsNull(traceSwitch);
            if (traceSwitch.TraceInfo)
            {
                Trace.TraceInformation(information);
            }
        }

        public static void TraceInformation(TraceSwitch traceSwitch, string format, params object[] args)
        {
            ValidateArgumentIsNull(traceSwitch);
            ValidateArgumentIsNull(args);
            if (traceSwitch.TraceInfo)
            {
                if (args != null && args.Length > 0)
                {
                    Trace.TraceInformation(format, args);
                }
                else
                {
                    Trace.TraceInformation(format);
                }
            }
        }

        public static void TraceWarning(TraceSwitch traceSwitch, string information)
        {
            ValidateArgumentIsNull(traceSwitch);
            if (traceSwitch.TraceWarning)
            {
                Trace.TraceWarning(information);
            }
        }

        public static void TraceWarning(TraceSwitch traceSwitch, string format, params object[] args)
        {
            ValidateArgumentIsNull(traceSwitch);
            ValidateArgumentIsNull(args);
            if (!traceSwitch.TraceWarning) return;
            
            if (args != null && args.Length > 0)
            {
                Trace.TraceWarning(format, args);
            }
            else
            {
                Trace.TraceWarning(format);
            }
        }


        public static void TraceVerbose(TraceSwitch traceSwitch, string information)
        {
            ValidateArgumentIsNull(traceSwitch);
            
            if (traceSwitch.TraceVerbose)
            {
                Trace.TraceInformation(information);
            }
        }

        public static void TraceVerbose(TraceSwitch traceSwitch, string format, params object[] args)
        {
            ValidateArgumentIsNull(traceSwitch);
            ValidateArgumentIsNull(args);
            if (!traceSwitch.TraceVerbose) return;
            
            if (args != null && args.Length > 0)
            {
                Trace.TraceInformation(format, args);
            }
            else
            {
                Trace.TraceInformation(format);
            }
        }

        private static void ValidateArgumentIsNull(TraceSwitch traceSwitch)
        {
            if (traceSwitch == null)
            {
                throw new ArgumentNullException("traceswitch");
            }
        }

        private static void ValidateArgumentIsNull(Exception ex)
        {
            if (ex== null)
            {
                throw new ArgumentNullException("ex");
            }
        }
        
        private static void ValidateArgumentIsNull(object[] args)        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
        }
        
    }
}
