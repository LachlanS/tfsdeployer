using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Web.Models
{
    public class EventListModel 
    {
        public IEnumerable<DeploymentEvent> RecentEvents { get; set; }
    }
}