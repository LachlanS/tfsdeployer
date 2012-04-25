using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace TfsDeployer
{
    internal class AppConfigTfsConnectionProvider : ITfsConnectionProvider
    {
        private readonly ICredentials _credentials;

        public AppConfigTfsConnectionProvider()
        {
            var settings = Properties.Settings.Default;
            if (string.IsNullOrEmpty(settings.TfsUserName)) return;

            _credentials = new NetworkCredential(settings.TfsUserName, settings.TfsPassword, settings.TfsDomain);
        }
        
        public TfsConnection GetConnection()
        {
            var uri = new Uri(Properties.Settings.Default.TeamProjectCollectionUri);
            if (_credentials == null)
            {
                return new TfsTeamProjectCollection(uri);
            }

            return new TfsTeamProjectCollection(uri, _credentials);
        }
    }
}