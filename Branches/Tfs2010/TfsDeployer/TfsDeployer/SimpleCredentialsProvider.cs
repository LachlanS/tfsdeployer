using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace TfsDeployer
{
    public class SimpleCredentialsProvider : ICredentialsProvider
    {
        private readonly ICredentials _credentials;

        public SimpleCredentialsProvider(ICredentials credentials)
        {
            _credentials = credentials;
        }

        public ICredentials GetCredentials(Uri uri, ICredentials failedCredentials)
        {
            return _credentials;
        }

        public ICredentials GetCredentials(Uri uri, ICredentials failedCredentials, string caption, string messageText)
        {
            return _credentials;
        }

        public void NotifyCredentialsAuthenticated(Uri uri)
        {

        }

    }
}