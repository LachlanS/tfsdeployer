namespace Readify.Useful.TeamFoundation.Common.Notification
{
    public abstract class NotificationService<TEventType> : INotificationService
    {
        protected abstract void OnNotificationEvent(TEventType eventRaised, TfsIdentity identity);
      
        public void Notify(string eventXml, string tfsIdentityXml)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch, "Notification Event Received Details as follows.\nEventXml:\n{0}\nTfsIdentityXml:\n{1}", eventXml, tfsIdentityXml);

            var eventReceived= ServiceHelper.DeserializeEvent<TEventType>(eventXml);
            var identity = ServiceHelper.DeserializeEvent<TfsIdentity>(tfsIdentityXml);
            OnNotificationEvent(eventReceived, identity);
        }

    }
}
