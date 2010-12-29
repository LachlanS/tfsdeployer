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
                    var binding = new WSHttpBinding {Security = {Mode = SecurityMode.None}};
                    var endpointAddress = new EndpointAddress("http://localhost/Temporary_Listen_Addresses/TfsDeployer/IDeployerService");
                    var deployerService = ChannelFactory<IDeployerService>.CreateChannel(binding, endpointAddress);
                    _uptimeText = deployerService.GetUptime().ToString();
                }
                return _uptimeText;
            }
        }
    }
}