using Readify.Useful.TeamFoundation.Common.Listener;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public class TfsBuildStatusTrigger
    {
        private delegate void ExecuteDeploymentProcessDelegate(BuildStatusChangeEvent ev);

        private readonly TfsListener _listener = new TfsListener();

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

        private static void OnListenerBuildStatusChangeEventReceived(object sender, BuildStatusChangeEventArgs e)
        {
            var deployer = new Deployer();
            ExecuteDeploymentProcessDelegate edpd = deployer.ExecuteDeploymentProcess;
            edpd.BeginInvoke(e.EventRaised, null, null);
        }
    }
}