using System;
using System.ServiceModel;
using Microsoft.TeamFoundation.Framework.Client;
using System.Diagnostics;

namespace Readify.Useful.TeamFoundation.Common.Notification
{

    public class NotificationServiceHost<TEventType> : ServiceHost where TEventType : new()
    {
        private readonly IEventService _eventService;
        private int _subscriptionId;

        public NotificationServiceHost(object singletonInstance, Uri baseAddress, IEventService eventService) 
            : base(singletonInstance, baseAddress)
        {
            _eventService = eventService;
        }

        public NotificationServiceHost(Type serviceType, Uri baseAddress, IEventService eventService)
            : base(serviceType, baseAddress)
        {
            _eventService = eventService;
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

        protected override void OnOpened()
        {
            Subscribe();
            base.OnOpened();
        }

        protected override void OnClosing()
        {
            Unsubscribe();
            base.OnClosing();
        }

        protected virtual void Unsubscribe()
        {
            if (_subscriptionId == 0) return;

            Trace.WriteLineIf(Constants.CommonSwitch.Level == TraceLevel.Verbose,
                              string.Format("Unsubscribing from Notification event id {0}", _subscriptionId),
                              Constants.NotificationServiceHost);
            
            _eventService.UnsubscribeEvent(_subscriptionId);
        }

        protected virtual void Subscribe()
        {
            var address = Description.Endpoints[0].Address.Uri.AbsoluteUri;
            Trace.WriteLineIf(Constants.CommonSwitch.Level == TraceLevel.Verbose,
                              string.Format("Subscribing to Notification event at address {0}", address),
                              Constants.NotificationServiceHost);
            var preference = CreateDeliveryPreference(address);

            _subscriptionId = _eventService.SubscribeEvent(typeof (TEventType).Name, null, preference);

            Trace.WriteLineIf(Constants.CommonSwitch.Level == TraceLevel.Verbose,
                              string.Format("Subscribed to Notification event at address {0} as id {1}", address, _subscriptionId),
                              Constants.NotificationServiceHost);
        }
      
    }
}
