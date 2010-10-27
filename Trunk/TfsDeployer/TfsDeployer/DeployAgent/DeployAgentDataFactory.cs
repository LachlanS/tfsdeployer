using System;
using System.Collections.Generic;
using System.Linq;
using TfsDeployer.Configuration;

namespace TfsDeployer.DeployAgent
{
    public class DeployAgentDataFactory
    {
        public DeployAgentData Create(string deployScriptRoot, Mapping mapping, BuildInformation buildInfo)
        {
            var data = new DeployAgentData
                           {
                               NewQuality = mapping.NewQuality,
                               OriginalQuality = mapping.OriginalQuality,
                               DeployServer = mapping.Computer,
                               DeployScriptFile = mapping.Script,
                               DeployScriptRoot = deployScriptRoot,
                               DeployScriptParameters = CreateParameters(mapping.ScriptParameters),
                               Timeout = mapping.TimeoutSeconds == 0 ? TimeSpan.MaxValue : TimeSpan.FromSeconds(mapping.TimeoutSeconds),
                               Tfs2005BuildData = buildInfo.Data,
                               Tfs2008BuildDetail = buildInfo.Detail
                           };
            return data;
        }

        private static ICollection<DeployScriptParameter> CreateParameters(IEnumerable<ScriptParameter> parameters)
        {
            if (parameters == null) return new List<DeployScriptParameter>();

            return parameters
                .Select(p => new DeployScriptParameter { Name = p.Name, Value = p.Value })
                .ToList();
        }

    }
}