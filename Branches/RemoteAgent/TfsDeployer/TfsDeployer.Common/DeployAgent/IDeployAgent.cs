using System.ServiceModel;

namespace TfsDeployer.DeployAgent
{
    [ServiceContract]
    public interface IDeployAgent
    {
        [OperationContract]
        DeployAgentResult Deploy(DeployAgentData deployAgentData);
    }
}