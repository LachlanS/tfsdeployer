using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.Build.Proxy;
using TfsDeployer.Runner;

namespace TfsDeployer.Notifier
{
    internal interface IAlert
    {
        void Alert(Mapping mapping, BuildData build, IRunner runner);
    }
}
