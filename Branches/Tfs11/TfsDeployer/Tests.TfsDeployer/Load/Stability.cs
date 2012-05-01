using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace Tests.TfsDeployer.Load
{
    public delegate void NotifyDelegate(string eventXml, string tfsIdentityXml, SubscriptionInfo subscriptionInfo);

    public class NotifyWcfClient : ClientBase<INotificationService>, INotificationService
    {
        public NotifyWcfClient(EndpointAddress remoteAddress)
            : base(new WSHttpBinding(SecurityMode.None), remoteAddress)
        { }

        public void Notify(string eventXml, string tfsIdentityXml, SubscriptionInfo subscriptionInfo)
        {
            Channel.Notify(eventXml, tfsIdentityXml, subscriptionInfo);
        }
    }

    [TestClass]
    public class Stability
    {
        [Ignore]
        [TestMethod]
        [TestCategory("Load")]
        public void Stability_should_not_crash_during_multiple_deployments()
        {
            // start TFS Deployer
            var startInfo = new ProcessStartInfo
                                {
                                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"dev\tfsdeployer\trunk\tfsdeployer\tfsdeployer\bin\debug\tfsdeployer.exe"),
                                    WindowStyle = ProcessWindowStyle.Normal //Hidden
                                };

            var deployerProcess = Process.Start(startInfo);

            // wait for TFS Deployer to be listening but how?? receive subscribeevent call?
            Thread.Sleep(3000);

            var asyncResults = new List<IAsyncResult>();
            var clients = new List<IDisposable>();

            var change = new BuildStatusChangeEvent
                             {
                                 TeamFoundationServerUrl = "http://MyTfsServer:8080/tfs/MyProjectCollection",
                                 TeamProject = "MyTeamProject",
                                 //<BuildUri>vstfs:///Build/Build/16</BuildUri>,
                                 Title = "MyTeamProject Build MyBuild_20110428.8 Quality Changed To Under Investigation",
                                 Subscriber = @"MyTfsServer\User",
                                 Id = "MyBuild_20110428.8",
                                 Url =
                                     "http://MyTfsServer:8080/tfs/web/build.aspx?pcguid=559aea18-2995-4d5e-a4d0-117e5f650ecf&amp;builduri=vstfs:///Build/Build/16",
                                 TimeZone = "AUS Eastern Daylight Time",
                                 TimeZoneOffset = "+11:00:00",
                                 ChangedTime = "8/12/2011 10:21:13 AM",
                                 StatusChange = new Change {FieldName = "Quality", NewValue = "Under Investigation"},
                                 ChangedBy = @"MyTfsServer\User"
                             };

            var changeSerializer = new XmlSerializer(typeof(BuildStatusChangeEvent));

            for (var i = 0; i < 100; i++)
            {
                var client =
                    new NotifyWcfClient(
                        new EndpointAddress(
                            "http://MyDeployerMachine/Temporary_Listen_Addresses/TfsDeployer/event/BuildStatusChangeEvent"));

                change.StatusChange.OldValue = string.Format("OldQuality{0}", i);
                
                string eventXml;
                using (var writer = new StringWriter())
                {
                    changeSerializer.Serialize(writer, change);
                    eventXml = writer.ToString();
                }

                var d = (NotifyDelegate) client.Notify;
                    var result = d.BeginInvoke(
                        eventXml, 
                        @"<TeamFoundationServer url=""http://MyTfsServer:8080/tfs/MyProjectCollection/Services/v3.0/LocationService.asmx"" />",
                        new SubscriptionInfo(), 
                        NotifyCallback,
                        d);
                    asyncResults.Add(result);
                
                clients.Add(client);
            }

            foreach (var result in asyncResults)
            {
                using (var waitHandle = result.AsyncWaitHandle)
                    waitHandle.WaitOne();
            }

            foreach(var client in clients)
            {
                client.Dispose();
            }

            Thread.Sleep(10000);

            // check process is still running
            try
            {
                Assert.IsFalse(deployerProcess.HasExited, "Process exited early");
            }
            finally
            {
                deployerProcess.Kill();
            }
        }

        void NotifyCallback(IAsyncResult result)
        {
            try
            {
                ((NotifyDelegate)result.AsyncState).EndInvoke(result);
            } 
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}