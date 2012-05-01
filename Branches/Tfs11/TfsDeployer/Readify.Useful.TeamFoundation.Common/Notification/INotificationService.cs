using System.ServiceModel;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
    [ServiceContract(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03")]
    public interface INotificationService
    {
        [OperationContract(Action = "*")] // support any action because TFS 11 Beta does not use the correct SOAP Action headers.
        [XmlSerializerFormat(Style = OperationFormatStyle.Document)] // Took me hours to figure this out!
        void Notify(string eventXml, string tfsIdentityXml, [MessageParameter(Name = "SubscriptionInfo")] SubscriptionInfo subscriptionInfo);

    }
}
