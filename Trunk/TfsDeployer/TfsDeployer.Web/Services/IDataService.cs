using System;
using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Web.Services
{
    public interface IDataService
    {
        IEnumerable<DeploymentEvent> GetRecentEvents(int maximumCount);
        TimeSpan GetUptime();
    }
}