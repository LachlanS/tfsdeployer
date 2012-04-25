using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.Journal;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class MappingExecutor : IMappingExecutor
    {
        private readonly IDeploymentEventRecorder _deploymentEventRecorder;
        private readonly IDeployAgentProvider _deployAgentProvider;
        private readonly IDeploymentFolderSource _deploymentFolderSource;
        private readonly NamedLockSet _namedLockSet;

        public MappingExecutor(IDeploymentEventRecorder deploymentEventRecorder, IDeployAgentProvider deployAgentProvider, IDeploymentFolderSource deploymentFolderSource, NamedLockSet namedLockSet)
        {
            _deploymentEventRecorder = deploymentEventRecorder;
            _deployAgentProvider = deployAgentProvider;
            _deploymentFolderSource = deploymentFolderSource;
            _namedLockSet = namedLockSet;
        }

        public void Execute(BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, Mapping mapping, IPostDeployAction postDeployAction, int deploymentId)
        {
            lock (_namedLockSet.GetLockObject(mapping.Queue))
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
                        deployData.DeploymentId = deploymentId;

                        _deploymentFolderSource.DownloadDeploymentFolder(deployData.TfsBuildDetail, workingDirectory.DirectoryInfo.FullName);
                        deployResult = deployAgent.Deploy(deployData);
                    }
                }

                postDeployAction.DeploymentFinished(mapping, deployResult);

                _deploymentEventRecorder.RecordFinished(deploymentId, deployResult.HasErrors, deployResult.Output);
            }
        }

        public void Dispose()
        {
            // noop
        }
    }
}