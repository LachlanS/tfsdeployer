using System.IO;
using TfsDeployer.DeployAgent;

namespace TfsDeployer.Agent
{
    public class DeployAgentService : IDeployAgent
    {
        private readonly ILog _log;

        public DeployAgentService(ILog log)
        {
            _log = log;
        }

        public DeployAgentResult Deploy(DeployAgentData deployAgentData)
        {
            var sourceDirectory = new DirectoryInfo(deployAgentData.DeployScriptRoot);
            using (var workingDirectory = new WorkingDirectory(_log))
            {
                RecursiveCopyContents(sourceDirectory, workingDirectory.DirectoryInfo);

                deployAgentData.DeployScriptRoot = sourceDirectory.FullName;

                var powerShellAgent = new LocalPowerShellDeployAgent();
                var result = powerShellAgent.Deploy(deployAgentData);
                return result;
            }
        }

        private void RecursiveCopyContents(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach(var sourceSubdirectory in source.GetDirectories())
            {
                var destSubdirectory = new DirectoryInfo(Path.Combine(destination.FullName, sourceSubdirectory.Name));
                if (!destSubdirectory.Exists)
                {
                    destSubdirectory.Create();
                }
                RecursiveCopyContents(sourceSubdirectory, destSubdirectory);
            }

            foreach(var sourceFile in source.GetFiles())
            {
                var destFile = new FileInfo(Path.Combine(destination.FullName, sourceFile.Name));
                sourceFile.CopyTo(destFile.FullName, true);
            }
        }
    }
}