using System;
using System.Collections.Generic;
using System.ServiceModel;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Wcf;
using Readify.Useful.TeamFoundation.Common;
using TfsDeployer.Data;

namespace TfsDeployer.Service
{
    public class DeployerServiceHost : IDisposable
    {
        private ServiceHost _host;

        public DeployerServiceHost(Uri baseAddress, ILifetimeScope lifetimeScope)
        {
            _host = new ServiceHost(typeof(DeployerService), baseAddress);
            var binding = new WSHttpBinding {Security = {Mode = SecurityMode.None}};
            var endpoint = _host.AddServiceEndpoint(typeof(IDeployerService), binding, typeof(IDeployerService).Name);
            var container = new ContainerAdapter(lifetimeScope);
            _host.AddDependencyInjectionBehavior<IDeployerService>(container);
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "DeployerService will listen at {0}", endpoint.ListenUri);
        }

        public void Start()
        {
            _host.Open();
        }

        public void Dispose()
        {
            var host = _host;
            _host = null;
            if (host == null) return;

            host.Close();
        }

        class ContainerAdapter : IContainer
        {
            private readonly ILifetimeScope _lifetimeScope;

            public ContainerAdapter(ILifetimeScope lifetimeScope)
            {
                _lifetimeScope = lifetimeScope;
            }

            public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
            {
                return _lifetimeScope.ResolveComponent(registration, parameters);
            }

            public IComponentRegistry ComponentRegistry
            {
                get { return _lifetimeScope.ComponentRegistry; }
            }

            public void Dispose()
            {
                _lifetimeScope.Dispose();
            }

            public ILifetimeScope BeginLifetimeScope()
            {
                return _lifetimeScope.BeginLifetimeScope();
            }

            public ILifetimeScope BeginLifetimeScope(object tag)
            {
                return _lifetimeScope.BeginLifetimeScope(tag);
            }

            public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
            {
                return _lifetimeScope.BeginLifetimeScope(configurationAction);
            }

            public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
            {
                return _lifetimeScope.BeginLifetimeScope(tag, configurationAction);
            }

            public IDisposer Disposer
            {
                get { return _lifetimeScope.Disposer; }
            }

            public object Tag
            {
                get { return _lifetimeScope.Tag; }
            }
        }

    }
}
