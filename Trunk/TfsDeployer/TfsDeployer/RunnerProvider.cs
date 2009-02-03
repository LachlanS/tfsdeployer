using TfsDeployer.DeployAgent;
using TfsDeployer.Runner;

namespace TfsDeployer
{
    public class RunnerProvider : IRunnerProvider
    {
        public IRunner GetRunner(Mapping map)
        {
            if (map.RunnerType == RunnerType.BatchFile)
            {
                return new BatchFileRunner();
            }
            return new RunnerToDeployAgentAdapter(new LocalPowerShellDeployAgent());
        }
    }
}