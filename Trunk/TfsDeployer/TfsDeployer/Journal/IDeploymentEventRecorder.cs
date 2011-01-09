namespace TfsDeployer.Journal
{
    public interface IDeploymentEventRecorder
    {
        int RecordTriggered(string buildNumber, string teamProject, string teamProjectCollectionUri, string triggeredBy, string originalQuality, string newQuality);
        int RecordMapped(int eventId, string script);
    }
}