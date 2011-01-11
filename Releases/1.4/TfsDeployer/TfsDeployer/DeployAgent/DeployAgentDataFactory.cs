using System;
using System.Collections.Generic;
using System.Linq;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer.DeployAgent
{
    public class DeployAgentDataFactory
    {
        public DeployAgentData Create(string deployScriptRoot, Mapping mapping, BuildDetail buildDetail, BuildStatusChangeEvent buildStatusChangeEvent)
        {
            var data = new DeployAgentData
                           {
                               NewQuality = buildStatusChangeEvent.StatusChange.NewValue,
                               OriginalQuality = buildStatusChangeEvent.StatusChange.OldValue,
                               DeployServer = mapping.Computer,
                               DeployScriptFile = mapping.Script,
                               DeployScriptRoot = deployScriptRoot,
                               DeployScriptParameters = CreateParameters(mapping.ScriptParameters),
                               Timeout = mapping.TimeoutSeconds == 0 ? TimeSpan.MaxValue : TimeSpan.FromSeconds(mapping.TimeoutSeconds),
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