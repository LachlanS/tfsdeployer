using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.Journal;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class MappingProcessor : IMappingProcessor
    {
        private static readonly object LocksLock = new object();
        private static readonly IDictionary Locks = new Hashtable();

        private delegate void ProcessMappingDelegate(BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, Mapping mapping, IPostDeployAction postDeployAction, int deploymentId);

        private readonly IDeployAgentProvider _deployAgentProvider;
        private readonly IDeploymentFolderSource _deploymentFolderSource;
        private readonly IMappingEvaluator _mappingEvaluator;
        private readonly IDeploymentEventRecorder _deploymentEventRecorder;

        public MappingProcessor(IDeployAgentProvider deployAgentProvider, IDeploymentFolderSource deploymentFolderSource, IMappingEvaluator mappingEvaluator, IDeploymentEventRecorder deploymentEventRecorder)
        {
            _deployAgentProvider = deployAgentProvider;
            _deploymentFolderSource = deploymentFolderSource;
            _mappingEvaluator = mappingEvaluator;
            _deploymentEventRecorder = deploymentEventRecorder;
        }

        public void ProcessMappings(IEnumerable<Mapping> mappings, BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, IPostDeployAction postDeployAction, int eventId)
        {
            var applicableMappings = from mapping in mappings
                                     where _mappingEvaluator.DoesMappingApply(mapping, statusChanged, buildDetail.Status.ToString())
                                     select mapping;

            foreach (var mapping in applicableMappings)
            {
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                                             "Matching mapping found, executing, Computer:{0}, Script:{1}",
                                             mapping.Computer,
                                             mapping.Script);

                var deploymentId = _deploymentEventRecorder.RecordQueued(eventId, mapping.Script, mapping.Queue);

                ((ProcessMappingDelegate)ProcessMapping).BeginInvoke(statusChanged, buildDetail, mapping, postDeployAction, deploymentId, null, null);
            }
        }

        private static object GetLockObject(Mapping mapping)
        {
            if (string.IsNullOrEmpty(mapping.Queue)) return new object();

            lock (LocksLock)
            {
                TraceHelper.TraceVerbose(TraceSwitches.TfsDeployer, "Providing lock object for queue: {0}", mapping.Queue);
                if (!Locks.Contains(mapping.Queue)) Locks.Add(mapping.Queue, new object());
                return Locks[mapping.Queue];
            }
        }
        
        private void ProcessMapping(BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, Mapping mapping, IPostDeployAction postDeployAction, int deploymentId)
        {
            lock(GetLockObject(mapping))
            {
                _deploymentEventRecorder.RecordStarted(deploymentId);
                
                var deployAgent = _deployAgentProvider.GetDeployAgent(mapping);

                // default to "happy; did nothing" if there's no deployment agent.
                var deployResult = new DeployAgentResult { HasErrors = false, Output = string.Empty };

                if (deployAgent != null)
                {
                    using (var workingDirectory = new WorkingDirectory())
                    {
                        var deployAgentDataFactory = new DeployAgentDataFactory();
                        var deployData = deployAgentDataFactory.Create(workingDirectory.DirectoryInfo.FullName,
                                                                       mapping, buildDetail, statusChanged);

                        _deploymentFolderSource.DownloadDeploymentFolder(deployData.TfsBuildDetail, workingDirectory.DirectoryInfo.FullName);
                        deployResult = deployAgent.Deploy(deployData);
                    }
                }

                postDeployAction.DeploymentFinished(mapping, deployResult);

                _deploymentEventRecorder.RecordFinished(deploymentId, deployResult.HasErrors, deployResult.Output);
            }
        }

    }
}