using TfsDeployer.Configuration;

namespace TfsDeployer.DeployAgent
{
    public class DeployAgentProvider : IDeployAgentProvider
    {
        public IDeployAgent GetDeployAgent(Mapping mapping)
        {
            if (string.IsNullOrEmpty(mapping.Script))
            {
                return null;
            }

            IDeployAgent agent;
            if (mapping.RunnerType == RunnerType.BatchFile)
            {
                agent = new BatchFileDeployAgent();
            }
            else
            {
                agent = new LocalPowerShellDeployAgent();
            }
            return agent;
        }
    }
}