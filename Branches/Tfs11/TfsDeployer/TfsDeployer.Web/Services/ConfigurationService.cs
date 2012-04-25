using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using TfsDeployer.Data;

namespace TfsDeployer.Web.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private static string StoragePath
        {
            get
            {
                var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TFSDeployer");
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }
                return Path.Combine(configPath, "DeployerEndpoints.txt");
            }
        }

        public string[] GetDeployerInstanceAddress()
        {
            if (File.Exists(StoragePath))
            {
                var fileContent = File.ReadAllText(StoragePath);
                var storedAddresses = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (storedAddresses.Any()) return storedAddresses;
            }

            return new[] { "http://localhost/Temporary_Listen_Addresses/TfsDeployer/IDeployerService" };
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

        public void SetDeployerInstanceAddress(string deployerServiceUrl)
        {
            var addresses = GetDeployerInstanceAddress();
            addresses[0] = deployerServiceUrl;
            File.WriteAllLines(StoragePath, addresses);
        }
    }
}