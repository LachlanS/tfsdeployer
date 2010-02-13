namespace TfsDeployer
{
    public interface IConfigurationSource
    {
        void CopyTo(string localPath);
    }
}