using System;
using System.ServiceModel;
using Microsoft.TeamFoundation.Framework.Client;
using System.Net;
using System.Diagnostics;

namespace Readify.Useful.TeamFoundation.Common.Notification
{

    /// <summary>
    /// Base class for the service hosts.
    /// </summary>
    public class NotificationServiceHost<TEventType> : ServiceHost where TEventType : new()
    {
        private readonly string _userName;
        private int _subscriptionId;

        public NotificationServiceHost(object singletonInstance, Uri baseAddress)
            : this(singletonInstance,baseAddress, CredentialCache.DefaultNetworkCredentials.UserName)
        { }

        public NotificationServiceHost(object singletonInstance, Uri baseAddress, string userName) : base(singletonInstance, baseAddress)
        {
            _userName = userName;
        }

        public NotificationServiceHost(Type serviceType, Uri baseAddress)
            : this(serviceType, baseAddress,CredentialCache.DefaultNetworkCredentials.UserName)
        { }

        public NotificationServiceHost(Type serviceType, Uri baseAddress, string userName) : base(serviceType, baseAddress)
        {
            _userName = userName;
        }

        private static DeliveryPreference CreateDeliveryPreference(string endPointAddress)
        {
            var preference = new DeliveryPreference
                                 {
                                     Address = endPointAddress,
                                     Type = DeliveryType.Soap,
                                     Schedule = DeliverySchedule.Immediate
                                 };
            return preference;
        }

        /// <summary>
        /// On opening the service host, subscribe to the event
        /// </summary>
        protected override void OnOpened()
        {
            Subscribe();
            base.OnOpened();
        }

        /// <summary>
        /// On closed, remove the event from the subscription
        /// </summary>
        protected override void OnClosing()
        {
            Unsubscribe();
            base.OnClosing();
        }

        /// <summary>
        /// Details how the event is subscribed 
        /// </summary>
        protected virtual void Unsubscribe()
        {
            Trace.WriteLineIf(Constants.CommonSwitch.Level == TraceLevel.Verbose,
                              string.Format("Unsubscribing from Notification event id {0}", _subscriptionId),
                              Constants.NotificationServiceHost);
            ServiceHelper.Unsubscribe(_subscriptionId);
        }

        protected virtual void Subscribe()
        {
            var address = Description.Endpoints[0].Address.Uri.AbsoluteUri;
            Trace.WriteLineIf(Constants.CommonSwitch.Level == TraceLevel.Verbose,
                              string.Format("Subscribing to Notification event at address {0}", address),
                              Constants.NotificationServiceHost);
            var preference = CreateDeliveryPreference(address);
            _subscriptionId = ServiceHelper.Subscribe(_userName, typeof (TEventType).Name, null, preference);
        }
      
    }
}
