using System.Web.UI;
using TfsDeployer.Data;
using WebShell;

namespace TfsDeployer.Web
{
    public partial class _Default : Page
    {
        public string UptimeText
        {
            get { return HostContext<IDeployerContext>.Current.Uptime.ToString(); }
        }
    }
}