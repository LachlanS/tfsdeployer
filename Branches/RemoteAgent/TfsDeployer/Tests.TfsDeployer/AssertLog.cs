using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;

namespace Tests.TfsDeployer
{
    internal class AssertLog : ILog
    {
        public void Information(string message)
        {
        }

        public void Warning(string message)
        {
        }

        public void Error(string message)
        {
            Assert.Fail(message);
        }
    }
}