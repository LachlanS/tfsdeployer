using System;
using System.ServiceModel;

namespace TfsDeployer.Data
{
    [ServiceContract(Namespace = "http://www.readify.net/TfsDeployer/DeployerService20101221")]
    public interface IDeployerService
    {
        [OperationContract]
        TimeSpan GetUptime();

        [OperationContract]
        DeploymentEvent[] RecentEvents(int count);

        [OperationContract]
        string GetDeploymentOutput(int deploymentId);
    }
}
