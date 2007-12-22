using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.Build.Proxy;

namespace TfsDeployer.Runner
{
    public interface IRunner
    {
        bool Execute(string directory, Mapping mapToRun,BuildData buildData);
        string Output
        {
            get;
        }

        bool ErrorOccured
        {
            get;
        }

        string ScriptRun
        {
            get;
        }
    }
}
