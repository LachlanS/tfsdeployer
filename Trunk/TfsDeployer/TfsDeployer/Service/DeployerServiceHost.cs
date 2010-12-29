using System;
using System.ServiceModel;
using Readify.Useful.TeamFoundation.Common;
using TfsDeployer.Data;

namespace TfsDeployer.Service
{
    public class DeployerServiceHost : IDisposable
    {
        private ServiceHost _host;

        public DeployerServiceHost(Uri baseAddress)
        {
            var service = new DeployerService();
            _host = new ServiceHost(service, baseAddress);
            var binding = new WSHttpBinding {Security = {Mode = SecurityMode.None}};
            var endpoint = _host.AddServiceEndpoint(typeof(IDeployerService), binding, typeof(IDeployerService).Name);
            _host.Open();
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "DeployerService listening at {0}", endpoint.ListenUri);
        }

        public void Dispose()
        {
            var host = _host;
            _host = null;
            if (host == null) return;

            host.Close();
        }
    }
}
