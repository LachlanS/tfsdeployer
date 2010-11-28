using System.Text.RegularExpressions;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public static class VersionControlPath
    {
        public static string GetDeploymentFolderServerPath(BuildDetail buildDetail)
        {
            return GetDeploymentFolderServerPath(buildDetail.BuildDefinition.Process.ServerPath);
        }

        private static string GetDeploymentFolderServerPath(string templateFile)
        {
            return Regex.Replace(templateFile, @"/[^/]+$", "/Deployment");
        }
    }
}