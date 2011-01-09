namespace TfsDeployer.Journal
{
    public interface IDeploymentEventRecorder
    {
        void RecordTriggered(string buildNumber, string teamProject, string teamProjectCollectionUri, string triggeredBy, string originalQuality, string newQuality);
    }
}