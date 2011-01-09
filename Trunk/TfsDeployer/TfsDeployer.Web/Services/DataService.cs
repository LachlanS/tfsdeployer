using System;
using System.Collections.Generic;
using System.ServiceModel;
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

        private IDeployerService GetDeployerService()
        {
            var endpointUri = _configurationService.GetDeployerInstanceAddress()[0];
            var binding = new WSHttpBinding { Security = { Mode = SecurityMode.None } };
            var endpointAddress = new EndpointAddress(endpointUri);
            return ChannelFactory<IDeployerService>.CreateChannel(binding, endpointAddress);
        }

    }
}