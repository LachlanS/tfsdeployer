using System;
using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Web.Services
{
    public class DataService : IDataService
    {
        private readonly IConfigurationService _configurationService;

        public DataService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public IEnumerable<DeploymentEvent> GetRecentEvents(int maximumEventCount)
        {
            return GetDeployerService().RecentEvents(maximumEventCount);
        }

        public TimeSpan GetUptime()
        {
            return GetDeployerService().GetUptime();
        }

        public string GetDeploymentOutput(int deploymentId)
        {
            return GetDeployerService().GetDeploymentOutput(deploymentId).Content;
        }

        private IDeployerService GetDeployerService()
        {
            return _configurationService.CreateDeployerService(0);
        }
    }
}