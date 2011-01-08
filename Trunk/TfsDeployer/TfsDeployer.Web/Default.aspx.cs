using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI;
using TfsDeployer.Data;

namespace TfsDeployer.Web
{
    public partial class _Default : Page
    {
        private string _uptimeText;

        public string UptimeText
        {
            get
            {
                if (_uptimeText == null)
                {
                    _uptimeText = GetDeployerService().GetUptime().ToString();
                }
                return _uptimeText;
            }
        }

        public IEnumerable<DeploymentEvent> RecentEvents
        {
            get { return GetDeployerService().RecentEvents(5); }
        }

        private static IDeployerService GetDeployerService()
        {
            var binding = new WSHttpBinding {Security = {Mode = SecurityMode.None}};
            var endpointAddress = new EndpointAddress("http://localhost/Temporary_Listen_Addresses/TfsDeployer/IDeployerService");
            return ChannelFactory<IDeployerService>.CreateChannel(binding, endpointAddress);
        }
    }
}