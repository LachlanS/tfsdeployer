namespace TfsDeployer
{
    public static class LogExtensions
    {
        public static void Information(this ILog log, string format, params object[] args)
        {
            log.Information(string.Format(format, args));
        }
        public static void Warning(this ILog log, string format, params object[] args)
        {
            log.Warning(string.Format(format, args));
        }
    }
}