using System.Diagnostics;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class DeployerContainerBuilderTests
    {
        [TestMethod]
        public void DeployerContainerBuilder_should_resolve_large_event_log_trace_listener_for_windows_service()
        {
            // Arrange 
            var builder = new DeployerContainerBuilder(DeployerContainerBuilder.RunMode.WindowsService);
            var container = builder.Build();

            // Act
            var traceListener = container.Resolve<TraceListener>();

            // Assert
            Assert.IsInstanceOfType(traceListener, typeof(LargeEventLogTraceListener));
        }

    }
}
