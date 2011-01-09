using System;
using System.Collections.Generic;
using System.Linq;
using TfsDeployer.Data;

namespace TfsDeployer.Web.Services
{
    public class DummyDeployerService : IDeployerService
    {
        public TimeSpan GetUptime()
        {
            return DateTime.UtcNow.Subtract(new DateTime(2011, 1, 10));
        }

        public DeploymentEvent[] RecentEvents(int count)
        {
            return GenerateDeploymentEvents(count).ToArray();
        }

        private static IEnumerable<DeploymentEvent> GenerateDeploymentEvents(int maxCount)
        {
            for (var eventCount = 0; eventCount < maxCount; eventCount++)
            {
                yield return new DeploymentEvent
                                 {
                                     BuildNumber = "MagicBuild.20110109." + eventCount,
                                     NewQuality = "Fantastic",
                                     OriginalQuality = "Less than desirable",
                                     QueuedDeployments = new[]
                                                             {
                                                                 new QueuedDeployment
                                                                     {
                                                                         Id = eventCount * 10,
                                                                         FinishedUtc = DateTime.Now,
                                                                         HasErrors = false,
                                                                         Queue = "Huh",
                                                                         QueuedUtc = DateTime.Now.AddMinutes(-eventCount),
                                                                         Script = "Deploy.ps1",
                                                                         StartedUtc = DateTime.Now.AddMinutes(-(eventCount + 1))
                                                                     }
                                                             },
                                     TeamProject = "Whoop, there it is",
                                     TeamProjectCollectionUri = "http://nowhere.com:8080/tfs/defaultcollection",
                                     TriggeredBy = "Jason 'PS1' Stangroome",
                                     TriggeredUtc = DateTime.UtcNow,
                                 };
            }
        }

        public string GetDeploymentOutput(int deploymentId)
        {
            return string.Format("Huzzah! Deployment output for {0}, updated at {1}", deploymentId, DateTime.UtcNow);
        }
    }
}