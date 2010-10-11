using System;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.TfsDeployer.TestHelpers;
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
        public void DeploymentHostUI_should_trace()
        {
            // Arrange
            var listener = new StringBuilderTraceListener();
            using (new TraceListenersScope())
            {
                Trace.Listeners.Add(listener);
                var hostUI = new DeploymentHostUI();
                try
                {
                    // Act
                    hostUI.ReadLine();
                }
                catch (Exception)
                {
                    // Swallow. We don't care about any result of ReadLine.
                }
            }

            // Assert
            Assert.IsTrue(0 <= listener.StringBuilder.ToString().IndexOf("ReadLine", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
