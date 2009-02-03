using TfsDeployer.DeployAgent;
using TfsDeployer.Runner;

namespace TfsDeployer
{
    public class RunnerProvider : IRunnerProvider
    {
        public IRunner GetRunner(Mapping map)
        {
            IDeployAgent agent;
            if (map.RunnerType == RunnerType.BatchFile)
            {
                agent = new BatchFileDeployAgent();
            }
            else
            {
                agent = new LocalPowerShellDeployAgent();
            }
            return new RunnerToDeployAgentAdapter(agent);
        }
    }
}