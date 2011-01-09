using System;
using System.IO;
using System.ServiceModel;
using TfsDeployer.Data;

namespace TfsDeployer.Web.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private static string StoragePath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"TFSDeployer\DeployerEndpoints.txt"); }
        }

        public string[] GetDeployerInstanceAddress()
        {
            if (!File.Exists(StoragePath)) return new[] { "http://localhost/Temporary_Listen_Addresses/TfsDeployer/IDeployerService" };

            var fileContent = File.ReadAllText(StoragePath);
            return fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public IDeployerService CreateDeployerService(int instanceIndex)
        {
            var endpointUri = GetDeployerInstanceAddress()[0];
            if (endpointUri.Equals(typeof(DummyDeployerService).Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return new DummyDeployerService();
            }

            var binding = new WSHttpBinding { Security = { Mode = SecurityMode.None } };
            var endpointAddress = new EndpointAddress(endpointUri);
            return ChannelFactory<IDeployerService>.CreateChannel(binding, endpointAddress);
        }
    }
}