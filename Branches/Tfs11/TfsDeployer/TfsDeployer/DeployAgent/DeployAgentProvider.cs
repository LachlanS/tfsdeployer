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
                var clrVersion = ClrVersion.Version2;
                if (mapping.RunnerTypeSpecified && mapping.RunnerType == RunnerType.PowerShellV3)
                {
                    clrVersion = ClrVersion.Version4;
                }
                agent = _componentContext.Resolve<OutOfProcessPowerShellDeployAgent>(new TypedParameter(typeof(ClrVersion), clrVersion));
            }
            return agent;
        }
    }
}