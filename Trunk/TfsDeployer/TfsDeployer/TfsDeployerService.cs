using System.ServiceProcess;
using System;

namespace TfsDeployer
{
    public class TfsDeployerService : ServiceBase
    {
        private readonly TfsDeployerApplication _application;

        public TfsDeployerService(Func<TfsDeployerApplication> createAppDelegate)
        {
            _application = createAppDelegate();
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