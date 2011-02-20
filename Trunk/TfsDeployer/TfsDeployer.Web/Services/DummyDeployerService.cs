using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var deployments = new Collection<QueuedDeployment>();
            for (var eventCount = 0; eventCount < maxCount; eventCount++)
            {
                var deploymentCount = deployments.Count();

                deployments.Add(new QueuedDeployment
                {
                    Id = deploymentCount * 10,
                    FinishedUtc = DateTime.Now.AddMinutes(-deploymentCount),
                    HasErrors = false,
                    Queue = "Queue " + deploymentCount,
                    QueuedUtc = DateTime.Now.AddMinutes(-deploymentCount + 2),
                    Script = "Deploy_" + deploymentCount + ".ps1",
                    StartedUtc = DateTime.Now.AddMinutes(-(deploymentCount + 1))
                });


                yield return new DeploymentEvent
                                 {
                                     BuildNumber = "MagicBuild.20110109." + eventCount,
                                     NewQuality = "Fantastic",
                                     OriginalQuality = "Less than desirable",
                                     QueuedDeployments = deployments.ToArray(),
                                     TeamProject = "Whoop, there it is",
                                     TeamProjectCollectionUri = "http://nowhere.com:8080/tfs/defaultcollection",
                                     TriggeredBy = "Jason 'PS1' Stangroome",
                                     TriggeredUtc = DateTime.UtcNow,
                                 };
            }
        }

        public DeploymentOutput GetDeploymentOutput(int deploymentId)
        {
            return new DeploymentOutput
                       {
                           Content =
                               string.Format("Huzzah! Deployment output for {0}, updated at {1}", deploymentId,
                                             DateTime.UtcNow),
                           IsFinal = (new Random()).Next(0, deploymentId) < 1
                       };
        }
    }
}