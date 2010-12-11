using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class MappingProcessor : IMappingProcessor
    {
        private delegate void ProcessMappingDelegate(BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, Mapping mapping, IPostDeployAction postDeployAction);

        private readonly IDeployAgentProvider _deployAgentProvider;
        private readonly IDeploymentFolderSource _deploymentFolderSource;
        private readonly IMappingEvaluator _mappingEvaluator;

        public MappingProcessor(IDeployAgentProvider deployAgentProvider, IDeploymentFolderSource deploymentFolderSource, IMappingEvaluator mappingEvaluator)
        {
            _deployAgentProvider = deployAgentProvider;
            _deploymentFolderSource = deploymentFolderSource;
            _mappingEvaluator = mappingEvaluator;
        }

        public void ProcessMappings(IEnumerable<Mapping> mappings, BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, IPostDeployAction postDeployAction)
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

                ((ProcessMappingDelegate)ProcessMapping).BeginInvoke(statusChanged, buildDetail, mapping, postDeployAction, null, null);
            }
        }

        private void ProcessMapping(BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, Mapping mapping, IPostDeployAction postDeployAction)
        {
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
        }

    }
}