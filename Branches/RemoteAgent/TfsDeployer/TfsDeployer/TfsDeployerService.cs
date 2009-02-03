using System.ServiceProcess;

namespace TfsDeployer
{
    public class TfsDeployerService : ServiceBase
    {
        private readonly TfsBuildStatusTrigger trigger = new TfsBuildStatusTrigger();

        protected override void OnStart(string[] args)
        {
            trigger.Start();
        }

        protected override void OnStop()
        {
            trigger.Stop();
        }
    }
}