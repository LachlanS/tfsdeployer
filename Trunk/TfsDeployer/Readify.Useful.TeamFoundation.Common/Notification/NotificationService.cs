using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Text;
using System;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
    public abstract class NotificationService<TEventType> : INotificationService
    {
        protected abstract void OnNotificationEvent(TEventType eventRaised, TfsIdentity identity);

        public void Notify(string eventXml, string tfsIdentityXml)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Notification Event Received Details as follows.\n\nEventXml:\n{0}\n\nTfsIdentityXml:\n{1}\n\n", eventXml, tfsIdentityXml);

            var eventReceived = DeserializeEvent<TEventType>(eventXml);
            var identity = DeserializeEvent<TfsIdentity>(tfsIdentityXml);

            TraceHelper.TraceInformation(Constants.CommonSwitch, "Event Properties:\n{0}\n", eventReceived.ToFieldDump());

            OnNotificationEvent(eventReceived, identity);
        }

        private static T DeserializeEvent<T>(string eventXml)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Deserializing Event of type {0} from XML:\n{1}", typeof(T), eventXml);
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(eventXml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

    }
}
