using System;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Listener;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Journal;

namespace TfsDeployer
{
    public class TfsBuildStatusTrigger
    {
        private delegate void ExecuteDeploymentProcessDelegate(BuildStatusChangeEvent ev, int eventId);

        private readonly ITfsListener _listener;
        private readonly Func<IDeployer> _deployerFactory;
        private readonly IDuplicateEventDetector _duplicateEventDetector;
        private readonly IDeploymentEventRecorder _deploymentEventRecorder;

        public TfsBuildStatusTrigger(ITfsListener listener, Func<IDeployer> deployerFactory, IDuplicateEventDetector duplicateEventDetector, IDeploymentEventRecorder deploymentEventRecorder)
        {
            _listener = listener;
            _deployerFactory = deployerFactory;
            _duplicateEventDetector = duplicateEventDetector;
            _deploymentEventRecorder = deploymentEventRecorder;
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
                var eventId = _deploymentEventRecorder.RecordTriggered(
                    changeEvent.Id, 
                    changeEvent.TeamProject, 
                    changeEvent.TeamFoundationServerUrl, 
                    changeEvent.ChangedBy,
                    changeEvent.StatusChange.OldValue, 
                    changeEvent.StatusChange.NewValue
                    );

                var deployer = _deployerFactory();
                ExecuteDeploymentProcessDelegate edpd = deployer.ExecuteDeploymentProcess;
                edpd.BeginInvoke(changeEvent, eventId, null, null);
            }
            else
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Received duplicate event '{0}' from TFS.", changeEvent.Title);
            }
        }
    }
}