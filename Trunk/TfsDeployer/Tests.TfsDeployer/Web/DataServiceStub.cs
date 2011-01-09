using System;
using System.Collections.Generic;
using TfsDeployer.Data;
using TfsDeployer.Web.Services;

namespace Tests.TfsDeployer.Web
{
    class DataServiceStub : IDataService
    {
        public IEnumerable<DeploymentEvent> GetRecentEvents(int maximumCount)
        {
            return GetDeploymentEvents(maximumCount);
        }

        public TimeSpan GetUptime()
        {
            return new TimeSpan(1, 2, 3, 4);
        }

        private static IEnumerable<DeploymentEvent> GetDeploymentEvents(int maxCount)
        {
            for (var eventCount = 0; eventCount < maxCount; eventCount++)
            {
                yield return new DeploymentEvent
                    {
                        BuildNumber = "MagicBuild.20110109." + eventCount,
                        NewQuality = "Radtastic",
                        OriginalQuality = "Crapola",
                        QueuedDeployments = new[]
                                                {
                                                    new QueuedDeployment
                                                        {
                                                            FinishedUtc = DateTime.Now,
                                                            HasErrors = false,
                                                            Queue = "Huh",
                                                            QueuedUtc = DateTime.Now.AddMinutes(-eventCount),
                                                            Script = "DumpIt.ps1",
                                                            StartedUtc = DateTime.Now.AddMinutes(-(eventCount + 1))
                                                        }
                                                },
                        TeamProject = "Whoop, there it is",
                        TeamProjectCollectionUri = "http://nowhere.com:8080/tfs/defaultcollection",
                        TriggeredBy = "Jason 'PS1' Stangroome",
                        TriggeredUtc = DateTime.Now,
                    };
            }
        }
    }
}
