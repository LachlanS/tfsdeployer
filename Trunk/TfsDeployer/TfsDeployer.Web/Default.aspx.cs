using System.Web.UI;

namespace TfsDeployer.Web
{
    public partial class _Default : Page
    {
        public string UptimeText
        {
            get
            {
                return "unknown"; // HostContext<IDeployerContext>.Current.Uptime.ToString(); 
            }
        }
    }
}