using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer.DeployAgent
{
    public class DeployAgentDataFactory
    {
        public DeployAgentData Create(string deployScriptRoot, Mapping mapping, BuildInformation buildInfo)
        {
            var buildDetail = new BuildDetail();
            PropertyAdapter.CopyProperties(typeof(IBuildDetail), buildInfo.Detail, typeof(BuildDetail), buildDetail);
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
                               TfsBuildDetail = buildDetail
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