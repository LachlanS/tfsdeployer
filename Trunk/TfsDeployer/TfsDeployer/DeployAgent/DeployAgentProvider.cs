using Autofac;
using TfsDeployer.Configuration;

namespace TfsDeployer.DeployAgent
{
    public class DeployAgentProvider : IDeployAgentProvider
    {
        private readonly IComponentContext _componentContext;

        public DeployAgentProvider(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

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
                agent = _componentContext.Resolve<OutOfProcessPowerShellDeployAgent>();
            }
            return agent;
        }
    }
}