using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Listener;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public class TfsBuildStatusTrigger
    {
        private delegate void ExecuteDeploymentProcessDelegate(BuildStatusChangeEvent ev);

        private readonly ITfsListener _listener;
        private readonly IDeployerFactory _deployerFactory;
        private readonly IDuplicateEventDetector _duplicateEventDetector;

        public TfsBuildStatusTrigger(ITfsListener listener, IDeployerFactory deployerFactory, IDuplicateEventDetector duplicateEventDetector)
        {
            _listener = listener;
            _deployerFactory = deployerFactory;
            _duplicateEventDetector = duplicateEventDetector;
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
            BuildStatusChangeEvent changeEvent = e.EventRaised;

            if (_duplicateEventDetector.IsUnique(changeEvent))
            {
                var deployer = _deployerFactory.Create();
                ExecuteDeploymentProcessDelegate edpd = deployer.ExecuteDeploymentProcess;
                edpd.BeginInvoke(changeEvent, null, null);
            }
            else
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Received duplicate event '{0}' from TFS.", changeEvent.Title);
            }
        }
    }
}