using System;
using System.ServiceModel.Channels;
using Microsoft.TeamFoundation.Framework.Client;
using Readify.Useful.TeamFoundation.Common.Notification;
using Readify.Useful.TeamFoundation.Common.Properties;
using System.ServiceModel;
using System.Xml;
using System.Diagnostics;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class TfsEventListener<T> : NotificationService<T>, ITfsEventListener where T : new()
    {
        public delegate void OnNotificationEventReceived(T eventRaised, TfsIdentity identity);

        //public static event EventHandler<NotificationEventArgs<T>> NotificationReceived;
        public static OnNotificationEventReceived NotificationDelegate { get; set; }

        private readonly TraceSwitch _traceSwitch = new TraceSwitch("TFSEventListener", string.Empty);
        private readonly IEventService _eventService;
        private NotificationServiceHost<T> _host;

        public TfsEventListener(IEventService eventService)
        {
            _eventService = eventService;
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
            // Setup a basic binding to use NTLM authentication.
            var quotas = new XmlDictionaryReaderQuotas
            {
                MaxArrayLength = int.MaxValue,
                MaxBytesPerRead = int.MaxValue,
                MaxDepth = int.MaxValue,
                MaxNameTableCharCount = int.MaxValue,
                MaxStringContentLength = int.MaxValue
            };

            var binding = new WSHttpBinding {ReaderQuotas = quotas};
            binding.Security.Mode = SecurityMode.None;

            return binding;
        }

        private NotificationServiceHost<T> CreateHostInstance()
        {
            var baseAddress = GetBaseAddress();
            var host = new NotificationServiceHost<T>(this, baseAddress, _eventService);
            AddHostEndpoint(host);
            return host;
        }

        private static Uri GetBaseAddress()
        {
            var baseAddress = Settings.Default.BaseAddress;
            if (!baseAddress.EndsWith("/") && !baseAddress.EndsWith(@"\"))
            {
                baseAddress += "/";
            }
            return new Uri(baseAddress + typeof(T).Name);
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
