using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.TfsDeployer.TestHelpers;
using TfsDeployer;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class LargeEventLogTraceListenerTests
    {
        [TestMethod]
        public void LargeEventLogTraceListener_should_handle_large_trace_messages_to_the_eventlog()
        {
            // Arrange
            using (new TraceListenersScope())
            {
                Trace.Listeners.Clear();
                Trace.Listeners.Add(new LargeEventLogTraceListener("TfsDeployer"));

                const string tinyMessage = "X";
                var largeMessage = new string('X', 0x8001);

                try
                {
                    Trace.TraceInformation(tinyMessage);
                } 
                catch(Exception ex)
                {
                    Assert.Inconclusive("Small trace message failed. Cannot test large message. Exception: {0}", ex);
                }

                // Act
                Trace.TraceInformation(largeMessage);
            }

            // Assert
            // no exceptions
        }
    }
}
