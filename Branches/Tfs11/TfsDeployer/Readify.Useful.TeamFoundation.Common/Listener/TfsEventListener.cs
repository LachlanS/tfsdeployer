using System;
using System.ServiceModel.Channels;
using Microsoft.TeamFoundation.Framework.Client;
using Readify.Useful.TeamFoundation.Common.Notification;
using System.ServiceModel;
using System.Xml;
using System.Diagnostics;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class TfsEventListener<T> : NotificationService<T>, ITfsEventListener where T : new()
    {
        public delegate void OnNotificationEventReceived(T eventRaised, TfsIdentity identity);

        public static OnNotificationEventReceived NotificationDelegate { get; set; }

        private readonly TraceSwitch _traceSwitch = new TraceSwitch("TFSEventListener", string.Empty);
        private readonly IEventService _eventService;
        private readonly Uri _baseAddress;
        private NotificationServiceHost<T> _host;

        public TfsEventListener(IEventService eventService, Uri baseAddress)
        {
            _eventService = eventService;
            _baseAddress = baseAddress;
        }

        public void Start()
        {
            OpenHost();
        }

        public void Stop()
        {
            _host.Close();
        }

        private void OpenHost()
        {
            _host = CreateHostInstance();
            try
            {
                _host.Open();
            }
            catch (Exception ex)
            {
                TraceHelper.TraceError(_traceSwitch, ex);
                throw;
            }
        }

        private static void AddHostEndpoint(ServiceHost host)
        {
            var binding = CreateHostBinding();
            host.AddServiceEndpoint(
                typeof (INotificationService),
                binding,
                typeof (T).Name
                );
        }

        private static Binding CreateHostBinding()
        {
            var quotas = new XmlDictionaryReaderQuotas
            {
                MaxArrayLength = int.MaxValue,
                MaxBytesPerRead = int.MaxValue,
                MaxDepth = int.MaxValue,
                MaxNameTableCharCount = int.MaxValue,
                MaxStringContentLength = int.MaxValue
            };

            // use BasicHttpBinding with SOAP 1.2 because TFS 11 Beta sends SOAP 1.2 message bodies with a SOAP 1.1 SOAPAction HTTP Header.
            var customBinding = new CustomBinding(new BasicHttpBinding { ReaderQuotas = quotas});
            var messageEncoding = customBinding.Elements.Find<TextMessageEncodingBindingElement>();
            messageEncoding.MessageVersion = MessageVersion.Soap12;
            return customBinding;
        }

        private NotificationServiceHost<T> CreateHostInstance()
        {
            var host = new NotificationServiceHost<T>(this, _baseAddress, _eventService);
            AddHostEndpoint(host);
            return host;
        }

        protected override void OnNotificationEvent(T eventRaised, TfsIdentity identity)
        {
            if (NotificationDelegate != null)
            {
                NotificationDelegate(eventRaised, identity);
            }
        }

    }
}
