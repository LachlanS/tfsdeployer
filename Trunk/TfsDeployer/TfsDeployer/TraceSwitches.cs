using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace TfsDeployer
{
    internal static class TraceSwitches
    {
        public static TraceSwitch TfsDeployer = new TraceSwitch("TfsDeployer", "TfsDeployer", "4");
    }
}
