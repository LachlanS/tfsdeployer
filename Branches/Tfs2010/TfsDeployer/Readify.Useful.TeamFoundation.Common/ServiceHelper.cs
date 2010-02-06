using System;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Client;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Readify.Useful.TeamFoundation.Common.Properties;


namespace Readify.Useful.TeamFoundation.Common
{
    /// <summary>
    /// This class contains utilities for in the communication 
    /// with a Team Foundation Server
    /// </summary>
    public static class ServiceHelper
    {


        #region Get Service
        
        public static T GetService<T>(string userName, string password, string domain)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "UserName: {0}", userName);
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Password: {0}", password);
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Domain: {0}", domain);

            NetworkCredential credential = new NetworkCredential(userName, password, domain);
            return GetService<T>(credential);
        }


        /// <summary>
        /// Get a Service from Team Foundation Server using the Default credientials,
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            if (Settings.Default.UseDefaultCredentials)
            {
                return GetService<T>(CredentialCache.DefaultCredentials);
            }
            
            return GetService<T>(Settings.Default.UserName, Settings.Default.Password, Settings.Default.Domain);
        }

        /// <summary>
        /// Get the server that we are connecting to
        /// </summary>
        public static Uri TeamFoundationServerUrl
        {
            get
            {
                return new Uri(Settings.Default.TeamFoundationServerUrl);
            }
        }

        /// <summary>
        /// Internal helper method to get the service
        /// </summary>
        /// <typeparam name="T">Type of service to retrieve</typeparam>
        /// <param name="credentials">Credentials to use when connecting the service</param>
        /// <returns></returns>
        private static T GetService<T>(ICredentials credentials)
        {
            TempCredentials creds = new TempCredentials(credentials);
            string teamFoundationServerUrl = Settings.Default.TeamFoundationServerUrl;
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Connecting to server {0} To get service {1} ", teamFoundationServerUrl, typeof(T).ToString());
            TeamFoundationServer server = TeamFoundationServerFactory.GetServer(teamFoundationServerUrl, creds);
            T service = (T)server.GetService(typeof(T));
            return service;
        }

        private class TempCredentials : ICredentialsProvider
        {
            private ICredentials _credentials;

            public TempCredentials(ICredentials credentials)
            {
                _credentials = credentials;
            }

            #region ICredentialsProvider Members

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

            #endregion
        }

        #endregion

        public static int Subscribe(string userName, TeamFoundationServerEvents eventToSubscribeTo,string filterExpression, DeliveryPreference preference)
        {
            
            return Subscribe(userName, eventToSubscribeTo.ToString(), filterExpression, preference);
        }

        public static int Subscribe(string userName, string eventToSubscribeTo, string filterExpression, DeliveryPreference preference)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Subscribing to {0} using UserName:{1} FilterExpression:{2}", eventToSubscribeTo, userName, filterExpression);
            IEventService service = GetService<IEventService>();
            return service.SubscribeEvent(userName, eventToSubscribeTo, filterExpression, preference);
        }
       
        public static void Unsubscribe(int subscriptionId)
        {
            IEventService service = GetService<IEventService>();
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Unsubscribing event with id {0}", subscriptionId);
            service.UnsubscribeEvent(subscriptionId);
        }

        /// <summary>
        /// Unsubscribe a number of subscriptions
        /// </summary>
        /// <param name="subscriptionIds"></param>
        public static void Unsubscribe(Collection<int> subscriptionIds)
        {
            foreach (int subscriptionId in subscriptionIds)
            {
                Unsubscribe(subscriptionId);
            }
        }

        /// <summary>
        /// This method deserializes and event that has been received into a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventXml"></param>
        /// <returns></returns>
        public static T DeserializeEvent<T>(string eventXml)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch,"Deserializing Event of type {0} from XML:\n{1}", typeof(T), eventXml);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(eventXml)) 
            {
                return (T)serializer.Deserialize(reader);
            }
            
        }

     
    }
}
