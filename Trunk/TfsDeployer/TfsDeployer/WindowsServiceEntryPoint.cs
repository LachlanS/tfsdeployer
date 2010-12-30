using System.ServiceProcess;

namespace TfsDeployer
{
    public class WindowsServiceEntryPoint : IProgramEntryPoint
    {
        private readonly TfsDeployerService _tfsDeployerService;

        public WindowsServiceEntryPoint(TfsDeployerService tfsDeployerService)
        {
            _tfsDeployerService = tfsDeployerService;
        }

        public void Run()
        {
            ServiceBase.Run(_tfsDeployerService);
        }
    }
}