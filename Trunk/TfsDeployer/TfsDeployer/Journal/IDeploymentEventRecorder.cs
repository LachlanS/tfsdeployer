namespace TfsDeployer.Journal
{
    public interface IDeploymentEventRecorder
    {
        int RecordTriggered(string buildNumber, string teamProject, string teamProjectCollectionUri, string triggeredBy, string originalQuality, string newQuality);
        int RecordQueued(int eventId, string script, string queue);
        void RecordStarted(int deploymentId);
        void RecordFinished(int deploymentId, bool hasErrors);
    }
}