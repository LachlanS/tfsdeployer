using System;
using Readify.Useful.TeamFoundation.Common.Notification;
using Readify.Useful.TeamFoundation.Common.Properties;
using System.ServiceModel;
using System.Xml;
using System.Diagnostics;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    
    internal class TfsEventListener<T>:NotificationServiceType<T>,ITfsEventListener where T : new()
    {
        readonly TraceSwitch _traceSwitch = new TraceSwitch("TFSEventListener", string.Empty);
        internal delegate void OnNotificationEventReceived(T eventRaised, TFSIdentity identity);
        private static OnNotificationEventReceived _onNotificationDelegate;
	    public static OnNotificationEventReceived NotificationDelegate
	    {
	      get { return _onNotificationDelegate;}
          set { _onNotificationDelegate = value;}
	    }
    	
        NotificationServiceHost<T> _host;
        public NotificationServiceHost<T> Host
        {
            get
            {
                return _host;
            }
        }


        public void Start()
        {
            OpenHost();
        }

        public void Stop()
        {
            _host.Close();
        }



        /// <summary>
        /// Open the WCF Host.  And a listener to 
        /// it's events.
        /// </summary>
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


        /// <summary>
        /// Add the endpoint for the checkin event to the host.
        /// </summary>
        /// <param name="host"></param>
        private void AddHostEndpoint(NotificationServiceHost<T> host)
        {
            BasicHttpBinding binding = CreateHostBinding();

            host.AddServiceEndpoint(typeof(INotificationService), binding, typeof(T).Name);
        }

        /// <summary>
        /// Create the host binding type
        /// </summary>
        /// <returns></returns>
        private static BasicHttpBinding CreateHostBinding()
        {
            // Setup a basic binding to use NTLM authentication.
            BasicHttpBinding binding = new BasicHttpBinding();
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();

            quotas.MaxArrayLength = int.MaxValue;
            quotas.MaxBytesPerRead = int.MaxValue;
            quotas.MaxDepth = int.MaxValue;
            quotas.MaxNameTableCharCount = int.MaxValue;
            quotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas = quotas;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            return binding;
        }

        /// <summary>
        /// Create the instance of the Notification Service.  
        /// </summary>
        /// <returns></returns>
        private NotificationServiceHost<T> CreateHostInstance()
        {

            Uri baseAddress = GetBaseAddress();
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

        private Uri GetBaseAddress()
        {
            string baseAddress;
            if (Settings.Default.BaseAddress.EndsWith("/") || Settings.Default.BaseAddress.EndsWith(@"\"))
            {
                baseAddress = Settings.Default.BaseAddress + typeof(T).Name;

            }
            else
            {

                baseAddress = Settings.Default.BaseAddress + "/" + typeof(T).Name;
            }
            return new Uri(baseAddress);
        }


        protected override void OnNotificationEvent(T eventRaised, TFSIdentity identity)
        {
            if (NotificationDelegate != null)
            {
                NotificationDelegate(eventRaised, identity);
            }
        }
    }
}
