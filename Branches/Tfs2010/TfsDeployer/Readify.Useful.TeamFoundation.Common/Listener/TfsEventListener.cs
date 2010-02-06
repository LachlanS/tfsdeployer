using System;
using Readify.Useful.TeamFoundation.Common.Notification;
using Readify.Useful.TeamFoundation.Common.Properties;
using System.ServiceModel;
using System.Xml;
using System.Diagnostics;

namespace Readify.Useful.TeamFoundation.Common.Listener
{

    internal class TfsEventListener<T> : NotificationService<T>, ITfsEventListener where T : new()
    {
        public delegate void OnNotificationEventReceived(T eventRaised, TfsIdentity identity);

        //public static event EventHandler<NotificationEventArgs<T>> NotificationReceived;
        public static OnNotificationEventReceived NotificationDelegate { get; set; }

        private readonly TraceSwitch _traceSwitch = new TraceSwitch("TFSEventListener", string.Empty);
        private NotificationServiceHost<T> _host;

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

        private static void AddHostEndpoint(NotificationServiceHost<T> host)
        {
            var binding = CreateHostBinding();
            host.AddServiceEndpoint(
                typeof (INotificationService),
                binding,
                typeof (T).Name
                );
        }

        private static BasicHttpBinding CreateHostBinding()
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

            var binding = new BasicHttpBinding {ReaderQuotas = quotas};
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

            return binding;
        }

        private NotificationServiceHost<T> CreateHostInstance()
        {
            var baseAddress = GetBaseAddress();
            NotificationServiceHost<T> host;
            if (!String.IsNullOrEmpty(Settings.Default.RegistrationUserName))
            {
                host = new NotificationServiceHost<T>(GetType(), baseAddress, Settings.Default.RegistrationUserName);
            }
            else
            {
                host = new NotificationServiceHost<T>(GetType(), baseAddress);
            }
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
