using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.TeamFoundation.Build.Proxy;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer.Configuration
{
    /// <summary>
    /// Responsible for reading the configuration items from whatever form it will take
    /// </summary>
    public static class ConfigurationReaderHelper
    {
        /// <summary>
        /// Read the config file from a stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private  static DeploymentMappings Read(TextReader reader)
        {
            
            XmlSerializer serializer = new XmlSerializer(typeof(DeploymentMappings));
            DeploymentMappings config = (DeploymentMappings)serializer.Deserialize(reader);
            reader.Close();
            return config;
        }

        public static DeploymentMappings Read(string configFileName)
        {
            //Verify that the deployment mappings file is a valid file
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Reading Configuration File:{0}", configFileName);

            
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Verifying Configuration File:{0}", configFileName);
            if (Properties.Settings.Default.SignDeploymentMappingFile)
            {
                if (!Encrypter.VerifyXml(configFileName,Properties.Settings.Default.KeyFile))
                {

                    TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Verification Failed for the deployment mapping file:{0} and key file {1}", configFileName, Properties.Settings.Default.KeyFile);
                    return null;
                }
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Verification Succeeded for the deployment mapping file:{0}", configFileName);

            }

            if (File.Exists(configFileName))
            {
                using (TextReader reader = new StreamReader(configFileName))
                {
                    DeploymentMappings config = Read(reader);
                    return config;
                }
            }
            else
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Reading Configuration File:{0} failed.", configFileName);

                return null;
            }
        }

      
        
    }
}
