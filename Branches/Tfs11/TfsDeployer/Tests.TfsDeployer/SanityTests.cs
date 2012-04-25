using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class SanityTests
    {
        [TestMethod]
        [TestCategory("LocalTfsInstance")]
        public void IEventService_should_return_the_same_subscription_id_for_duplicate_subscriptions()
        {
            // arrange
            var collection = new TfsTeamProjectCollection(new Uri("http://localhost:8080/tfs/DefaultCollection"));
            var eventService = collection.GetService<IEventService>();
            var deliveryPreference = new DeliveryPreference
                                         {
                                             Address = string.Format("http://foo/{0}", GetType().FullName),
                                             Schedule = DeliverySchedule.Immediate,
                                             Type = DeliveryType.Soap
                                         };

            var firstSubscriptionId = 0;
            var secondSubscriptionId = 0;
            try
            {
                firstSubscriptionId = eventService.SubscribeEvent("BuildStatusChangeEvent", null, deliveryPreference);

                // act
                secondSubscriptionId = eventService.SubscribeEvent("BuildStatusChangeEvent", null, deliveryPreference);
            }
            finally
            {
                // absterge
                try { eventService.UnsubscribeEvent(firstSubscriptionId); } catch { }
                try { eventService.UnsubscribeEvent(secondSubscriptionId); } catch { }
            }

            // assert
            Assert.AreNotEqual(0, secondSubscriptionId);
            Assert.AreEqual(firstSubscriptionId, secondSubscriptionId);
        }

   }
}
