using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class PostDeployAction : IPostDeployAction
    {
        private readonly BuildDetail _buildDetail;
        private readonly IBuildDetail _tfsBuildDetail;
        private readonly IAlert _alert;

        public PostDeployAction(BuildDetail buildDetail, IBuildDetail tfsBuildDetail, IAlert alert)
        {
            _buildDetail = buildDetail;
            _tfsBuildDetail = tfsBuildDetail;
            _alert = alert;
        }

        public void DeploymentFinished(Mapping mapping, DeployAgentResult result)
        {
            ApplyRetainBuild(mapping, result, _tfsBuildDetail);
            _alert.Alert(mapping, _buildDetail, result);
        }

        private static void ApplyRetainBuild(Mapping mapping, DeployAgentResult deployAgentResult, IBuildDetail detail)
        {
            if (!mapping.RetainBuildSpecified) return;
            if (deployAgentResult.HasErrors) return;

            // no change to setting?
            if (detail.KeepForever == mapping.RetainBuild) return;

            detail.KeepForever = mapping.RetainBuild;
            try
            {
                detail.Save();
            }
            catch (AccessDeniedException ex)
            {
                deployAgentResult.Output = string.Format("{0}\n{1}", deployAgentResult.Output, ex);
            }
        }
    }
}