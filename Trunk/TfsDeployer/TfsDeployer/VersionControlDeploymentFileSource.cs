using System.IO;
using Microsoft.TeamFoundation.VersionControl.Client;
using Readify.Useful.TeamFoundation.Common;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class VersionControlDeploymentFileSource : IDeploymentFileSource
    {
        private readonly VersionControlServer _versionControlServer;

        public VersionControlDeploymentFileSource(VersionControlServer versionControlServer)
        {
            _versionControlServer = versionControlServer;
        }

        public Stream DownloadDeploymentFile(BuildDetail buildDetail)
        {
            var deploymentFile = GetDeploymentMappingsFileServerPath(buildDetail);
            var itemSpec = new ItemSpec(deploymentFile, RecursionType.None);
            var itemSet = _versionControlServer.GetItems(itemSpec, VersionSpec.Latest, DeletedState.NonDeleted, ItemType.File, GetItemsOptions.Download);
            if (itemSet.Items.Length == 0)
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Could not download file {0} from version control.", deploymentFile);
                return null;
            }
            return itemSet.Items[0].DownloadFile();
        }

        private static string GetDeploymentMappingsFileServerPath(BuildDetail buildDetail)
        {
            var folder = VersionControlPath.GetDeploymentFolderServerPath(buildDetail);
            return folder + "/DeploymentMappings.xml";
        }
    }
}