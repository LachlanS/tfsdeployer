using System;
using System.Collections.Generic;
using System.Text;
using Genghis;
using System.IO;

namespace TfsDeployer
{
    public class CommandLine:CommandLineParser
    {
        [ValueUsage("Deployment Mapping File Name", Name = "m", Optional = true)]
        public string DeploymentMappingFileName;

        [ValueUsage("Key File Name", Name = "k", Optional = true)]
        public string KeyFileName;

        [ValueUsage("Create Key File", Name = "g", Optional = true)]
        public string CreateKeyFileName;

        public bool CreateKeyFile
        {
            get
            {
                return !string.IsNullOrEmpty(CreateKeyFileName);
            }
        }
        public bool EncyptDeploymentFile
        {
            get
            {
                return !string.IsNullOrEmpty(DeploymentMappingFileName);
            }
        }


        protected override void Parse(string[] args, bool ignoreFirstArg)
        {
            base.Parse(args, ignoreFirstArg);
            if (!string.IsNullOrEmpty(CreateKeyFileName) && !string.IsNullOrEmpty(KeyFileName))
            {
                throw new ApplicationException("Key File and Create Key File cannot be specified at the same time");
            }

            if (!CreateKeyFile && (!File.Exists(KeyFileName)))
            {
                throw new ArgumentOutOfRangeException("Key File Name",KeyFileName,"Cannot find the specified key file.");
            }

            if (!CreateKeyFile && !File.Exists(DeploymentMappingFileName))
            {
                throw new ArgumentOutOfRangeException("Deploymnet Mapping File", DeploymentMappingFileName, "Deploymentpping file is missing");
            }
        }
        
        

    }
}
