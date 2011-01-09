using System;
using System.IO;

namespace TfsDeployer.Web.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private static string StoragePath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TFSDeployer\\DeployerEndpoints.txt"); }
        }

        public string[] GetDeployerInstanceAddress()
        {
            if (!File.Exists(StoragePath)) return new[] { "http://localhost/Temporary_Listen_Addresses/TfsDeployer/IDeployerService" };

            var fileContent = File.ReadAllText(StoragePath);
            return fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}