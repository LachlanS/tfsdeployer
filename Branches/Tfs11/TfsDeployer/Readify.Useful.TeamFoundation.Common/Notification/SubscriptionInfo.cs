using System;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
    [Serializable]
    public class SubscriptionInfo
    {
        public string Classification { get; set; }
        public int ID { get; set; }
        public string Subscriber { get; set; }
    }
}