using System;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.DeploymentHostUITests
{
    [TestClass]
    public class When_ReadLine_is_called
    {
        class StringBuilderTraceListener : TraceListener
        {
            public readonly StringBuilder StringBuilder = new StringBuilder();

            public override void Write(string message)
            {
                StringBuilder.Append(message);
            }

            public override void WriteLine(string message)
            {
                StringBuilder.AppendLine(message);
            }
        }

        [TestMethod]
        public void Should_trace()
        {
            var listener = new StringBuilderTraceListener();
            Trace.Listeners.Add(listener);
            var hostUI = new DeploymentHostUI();
            try
            {
                hostUI.ReadLine();
            } 
            catch (Exception)
            {
                // Swallow. We don't care about any result of ReadLine.
            }
            Trace.Listeners.Remove(listener);
            Assert.IsTrue(0 <= listener.StringBuilder.ToString().IndexOf("ReadLine", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
