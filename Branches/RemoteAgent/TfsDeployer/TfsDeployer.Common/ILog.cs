namespace TfsDeployer
{
    public interface ILog
    {
        void Information(string message);
        void Warning(string message);
        void Error(string message);
    }
}
