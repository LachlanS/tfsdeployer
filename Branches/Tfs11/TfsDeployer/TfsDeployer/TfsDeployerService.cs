using System.ServiceProcess;

namespace TfsDeployer
{
    public class TfsDeployerService : ServiceBase
    {
        private readonly TfsDeployerApplication _application;

        public TfsDeployerService(TfsDeployerApplication application)
        {
            _application = application;
        }

        protected override void OnStart(string[] args)
        {
            _application.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _application.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}