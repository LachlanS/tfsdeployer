using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Web.Models
{
    public class EventListModel 
    {
        public string UptimeText { get; set; }

        public IEnumerable<DeploymentEvent> RecentEvents { get; set; }
    }
}