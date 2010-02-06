using Microsoft.TeamFoundation.Framework.Client;
using Readify.Useful.TeamFoundation.Common.Listener;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public class TfsBuildStatusTrigger
    {
        private delegate void ExecuteDeploymentProcessDelegate(BuildStatusChangeEvent ev);

        private readonly TfsListener _listener;
        private readonly IDeployerFactory _deployerFactory;

        public TfsBuildStatusTrigger(IEventService eventService, IDeployerFactory deployerFactory)
        {
            _listener = new TfsListener(eventService);
            _deployerFactory = deployerFactory;
        }

        public void Start()
        {
            _listener.BuildStatusChangeEventReceived += OnListenerBuildStatusChangeEventReceived;
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.BuildStatusChangeEventReceived -= OnListenerBuildStatusChangeEventReceived;
        }

        private void OnListenerBuildStatusChangeEventReceived(object sender, BuildStatusChangeEventArgs e)
        {
            var deployer = _deployerFactory.Create();
            ExecuteDeploymentProcessDelegate edpd = deployer.ExecuteDeploymentProcess;
            edpd.BeginInvoke(e.EventRaised, null, null);
        }
    }
}