using System.Reflection;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.Journal;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class DeployAgentProviderTests
    {
        [TestMethod]
        public void DeployAgentProvider_should_provide_nothing_for_an_unspecified_script()
        {
            var mapping = new Mapping {Script = string.Empty};

            var provider = new DeployAgentProvider(null);
            var agent = provider.GetDeployAgent(mapping);

            Assert.IsNull(agent);

        }

        [TestMethod]
        public void DeployAgentProvider_should_provide_a_BatchFileDeployAgent_for_BatchFile_runner_type()
        {
            var mapping = new Mapping {RunnerType = RunnerType.BatchFile, Script = "not blank"};

            var provider = new DeployAgentProvider(null);
            var agent = provider.GetDeployAgent(mapping);

            Assert.IsInstanceOfType(agent, typeof(BatchFileDeployAgent));

        }

        [TestMethod]
        public void DeployAgentProvider_should_provide_an_OutOfProcessPowerShellDeployAgent_for_unspecified_runner_type()
        {
            var mapping = new Mapping {RunnerTypeSpecified = false, Script = "not blank"};

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<DeploymentEventJournal>().As<IDeploymentEventRecorder>();
            containerBuilder.RegisterType<OutOfProcessPowerShellDeployAgent>();

            using (var container = containerBuilder.Build())
            {
                var provider = new DeployAgentProvider(container);
                var agent = provider.GetDeployAgent(mapping);

                Assert.IsInstanceOfType(agent, typeof(OutOfProcessPowerShellDeployAgent));
            }

        }

        [TestMethod]
        public void DeployAgentProvider_should_provide_an_OutOfProcessPowerShellDeployAgent_with_ClrVersion4_for_PowerShellV3_runner_type()
        {
            var mapping = new Mapping { RunnerType = RunnerType.PowerShellV3, RunnerTypeSpecified = true, Script = "not blank" };

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<DeploymentEventJournal>().As<IDeploymentEventRecorder>();
            containerBuilder.RegisterType<OutOfProcessPowerShellDeployAgent>();

            IDeployAgent agent;
            using (var container = containerBuilder.Build())
            {
                var provider = new DeployAgentProvider(container);
                agent = provider.GetDeployAgent(mapping);
            }

            Assert.IsInstanceOfType(agent, typeof(OutOfProcessPowerShellDeployAgent));

            var clrVersion = (ClrVersion)agent.GetType().GetField("_clrVersion", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(agent);
            Assert.AreEqual(ClrVersion.Version4, clrVersion);
        }


    }

}