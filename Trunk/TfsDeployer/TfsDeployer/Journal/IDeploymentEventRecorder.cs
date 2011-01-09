namespace TfsDeployer.Journal
{
    public interface IDeploymentEventRecorder
    {
        int RecordTriggered(string buildNumber, string teamProject, string teamProjectCollectionUri, string triggeredBy, string originalQuality, string newQuality);
        int RecordQueued(int eventId, string script);
    }
}