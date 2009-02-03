namespace TfsDeployer.DeployAgent
{
    public class DeployAgentProvider : IDeployAgentProvider
    {
        public IDeployAgent GetDeployAgent(Mapping mapping)
        {
            IDeployAgent agent;
            if (mapping.RunnerType == RunnerType.BatchFile)
            {
                agent = new BatchFileDeployAgent(new TraceSwitchLog(TraceSwitches.TfsDeployer));
            }
            else
            {
                agent = new LocalPowerShellDeployAgent();
            }
            return agent;
        }
    }
}