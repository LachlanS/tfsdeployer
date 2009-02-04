using System.ServiceModel;

namespace TfsDeployer.Agent
{
    public class DeployAgentListener
    {
        private readonly ServiceHost _host;

        public DeployAgentListener()
        {
            _host = new ServiceHost(typeof(DeployAgentService));
        }

        public void Start()
        {
            _host.Open();
        }

        public void Stop()
        {
            _host.Close();
        }
    }
}
