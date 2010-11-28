using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Readify.Useful.TeamFoundation.Common.Listener;
using TfsDeployer.Configuration;

namespace TfsDeployer
{
    public class TfsDeployerApplication : IDisposable
    {
        private TfsBuildStatusTrigger _trigger;

        public void Start()
        {
            if (_trigger != null) throw new InvalidOperationException("Already started.");

            var serverProvider = new AppConfigTfsConnectionProvider();
            var server = serverProvider.GetConnection();
            var eventService = server.GetService<IEventService>();
            var buildServer = server.GetService<IBuildServer>();
            var versionControlServer = server.GetService<VersionControlServer>();
            var configurationReader = new ConfigurationReader(new VersionControlDeploymentFileSource(versionControlServer), Properties.Settings.Default.KeyFile);
            var deploymentFolderSource = new VersionControlDeploymentFolderSource(versionControlServer);
            var baseAddress = new Uri(Properties.Settings.Default.BaseAddress);
            var listener = new TfsListener(eventService, baseAddress);
            var duplicateEventDetector = new DuplicateEventDetector();

            _trigger = new TfsBuildStatusTrigger(listener, new DeployerFactory(buildServer, configurationReader, deploymentFolderSource), duplicateEventDetector);
            _trigger.Start();
        }

        #region IDisposable Members

        private bool _isDisposed = false;

        ~TfsDeployerApplication()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!_isDisposed)
            {
                if (disposeManagedResources)
                {
                    try
                    {
                        if (_trigger != null)
                        {
                            _trigger.Stop();
                            _trigger = null;
                        }
                    }
                    catch (Exception)
                    {
                        // swallow. Clean up no matter what.
                    }
                }

                _isDisposed = true;
            }
        }

        #endregion
    }
}