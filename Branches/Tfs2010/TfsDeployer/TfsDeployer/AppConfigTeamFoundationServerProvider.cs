using System.Net;
using Microsoft.TeamFoundation.Client;

namespace TfsDeployer
{
    internal class AppConfigTeamFoundationServerProvider : ITeamFoundationServerProvider
    {
        private readonly ICredentialsProvider _credentialsProvider;

        public AppConfigTeamFoundationServerProvider()
        {
            var settings = Properties.Settings.Default;
            if (string.IsNullOrEmpty(settings.TfsUserName)) return;

            var credentials = new NetworkCredential(settings.TfsUserName, settings.TfsPassword, settings.TfsDomain);
            _credentialsProvider = new SimpleCredentialsProvider(credentials);
        }
        
        public TeamFoundationServer GetServer()
        {
            return TeamFoundationServerFactory.GetServer(Properties.Settings.Default.TeamProjectCollectionUri, _credentialsProvider);
        }
    }
}