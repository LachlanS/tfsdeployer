using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Build.Client;

namespace TfsDeployer
{
    public static class VersionControlPath
    {
        public static string GetDeploymentFolderServerPath(IBuildDetail buildDetail)
        {
            var templateFile = buildDetail.BuildDefinition.Process.ServerPath;
            return Regex.Replace(templateFile, @"/[^/]+$", "/Deployment");
        }
    }
}