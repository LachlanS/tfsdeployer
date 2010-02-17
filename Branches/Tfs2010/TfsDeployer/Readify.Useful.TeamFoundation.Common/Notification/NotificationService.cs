using System.IO;
using System.Xml.Serialization;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
    public abstract class NotificationService<TEventType> : INotificationService
    {
        protected abstract void OnNotificationEvent(TEventType eventRaised, TfsIdentity identity);
      
        public void Notify(string eventXml, string tfsIdentityXml)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Notification Event Received Details as follows.\nEventXml:\n{0}\nTfsIdentityXml:\n{1}", eventXml, tfsIdentityXml);

            var eventReceived= DeserializeEvent<TEventType>(eventXml);
            var identity = DeserializeEvent<TfsIdentity>(tfsIdentityXml);
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
