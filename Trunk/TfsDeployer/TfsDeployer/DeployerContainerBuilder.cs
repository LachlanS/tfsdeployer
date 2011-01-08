using System;
using System.Diagnostics;
using Autofac;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Readify.Useful.TeamFoundation.Common.Listener;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.Data;
using TfsDeployer.DeployAgent;
using TfsDeployer.Properties;
using TfsDeployer.Service;

namespace TfsDeployer
{
    public class DeployerContainerBuilder
    {
        private readonly ContainerBuilder _containerBuilder;

        public enum RunMode
        {
            WindowsService,
            InteractiveConsole
        }

        public DeployerContainerBuilder(RunMode mode)
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<TfsDeployerApplication>().SingleInstance();
            _containerBuilder.RegisterType<TfsDeployerService>().SingleInstance();

            _containerBuilder.RegisterType<AppConfigTfsConnectionProvider>().As<ITfsConnectionProvider>();
            _containerBuilder.Register(c => c.Resolve<ITfsConnectionProvider>().GetConnection()).InstancePerLifetimeScope();
            _containerBuilder.Register(c => c.Resolve<TfsConnection>().GetService<IEventService>());
            _containerBuilder.Register(c => c.Resolve<TfsConnection>().GetService<IBuildServer>());
            _containerBuilder.Register(c => c.Resolve<TfsConnection>().GetService<VersionControlServer>());

            _containerBuilder.RegisterType<VersionControlDeploymentFileSource>().As<IDeploymentFileSource>();
            _containerBuilder.RegisterType<VersionControlDeploymentFolderSource>().As<IDeploymentFolderSource>();
            _containerBuilder.RegisterType<ConfigurationReader>().As<IConfigurationReader>()
                .WithParameter("signingKeyFile", Settings.Default.KeyFile);

            _containerBuilder.RegisterType<DuplicateEventDetector>().As<IDuplicateEventDetector>();

            _containerBuilder.RegisterType<DeployAgentProvider>().As<IDeployAgentProvider>();
            _containerBuilder.RegisterType<EmailAlerter>().As<IAlert>();
            _containerBuilder.RegisterType<MappingEvaluator>().As<IMappingEvaluator>();
            _containerBuilder.RegisterType<MappingProcessor>().As<IMappingProcessor>();
            _containerBuilder.RegisterType<Deployer>().As<IDeployer>();

            _containerBuilder.RegisterType<PostDeployAction>().As<IPostDeployAction>();

            var listenPrefix = Settings.Default.BaseAddress;
            if (!listenPrefix.EndsWith("/")) listenPrefix += "/";
            _containerBuilder.RegisterType<TfsListener>().As<ITfsListener>()
                .SingleInstance()
                .WithParameter("baseAddress", new Uri(listenPrefix + "event/"));

            _containerBuilder.RegisterType<TfsBuildStatusTrigger>();
            _containerBuilder.RegisterType<DeployerService>().As<IDeployerService>();
            _containerBuilder.RegisterType<DeployerServiceHost>()
                .SingleInstance()
                .WithParameter("baseAddress", new Uri(listenPrefix));

            switch (mode)
            {
                case RunMode.InteractiveConsole:
                    {
                        _containerBuilder.RegisterType<ConsoleTraceListener>().As<TraceListener>();
                        _containerBuilder.RegisterType<ConsoleEntryPoint>().As<IProgramEntryPoint>().SingleInstance();
                        break;
                    }
                case RunMode.WindowsService:
                    {
                        _containerBuilder.Register(c => new LargeEventLogTraceListener("TfsDeployer")).As<TraceListener>();
                        _containerBuilder.RegisterType<WindowsServiceEntryPoint>().As<IProgramEntryPoint>().SingleInstance();
                        break;
                    }
            }
            
        }

        public ContainerBuilder InnerBuilder
        {
            get { return _containerBuilder; }
        }

        public IContainer Build()
        {
            return _containerBuilder.Build();
        }
    }
}