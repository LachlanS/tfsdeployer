using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace TfsDeployer
{
    internal class AppConfigTfsConnectionProvider : ITfsConnectionProvider
    {
        private readonly ICredentialsProvider _credentialsProvider;

        public AppConfigTfsConnectionProvider()
        {
            var settings = Properties.Settings.Default;
            if (string.IsNullOrEmpty(settings.TfsUserName)) return;

            var credentials = new NetworkCredential(settings.TfsUserName, settings.TfsPassword, settings.TfsDomain);
            _credentialsProvider = new SimpleCredentialsProvider(credentials);
        }
        
        public TfsConnection GetConnection()
        {
            var uri = new Uri(Properties.Settings.Default.TeamProjectCollectionUri);
            return TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri, _credentialsProvider);
        }
    }
}