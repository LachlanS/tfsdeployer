using System.Collections.Generic;
using System.IO;
using TfsDeployer.Runner;

namespace TfsDeployer.DeployAgent
{
    public class RunnerToDeployAgentAdapter : IRunner
    {
        private readonly IDeployAgent _deployAgent;

        public RunnerToDeployAgentAdapter(IDeployAgent deployAgent)
        {
            _deployAgent = deployAgent;
        }

        public bool Execute(string directory, Mapping mapToRun, BuildInformation buildInfo)
        {
            var data = new DeployAgentData
                           {
                               NewQuality = mapToRun.NewQuality,
                               OriginalQuality = mapToRun.OriginalQuality,
                               DeployServer = mapToRun.Computer,
                               DeployScriptFile = mapToRun.Script,
                               DeployScriptRoot = directory,
                               DeployScriptParameters = CreateParameters(mapToRun.ScriptParameters),
                               Tfs2005BuildData = buildInfo.Data,
                               Tfs2008BuildDetail = buildInfo.Detail
                           };

            var result = _deployAgent.Deploy(data);

            ScriptRun = Path.Combine(data.DeployScriptRoot, data.DeployScriptFile);
            ErrorOccurred = result.HasErrors;
            Output = result.Output;
            return ErrorOccurred;
        }

        private static ICollection<DeployScriptParameters> CreateParameters(IEnumerable<ScriptParameter> parameters)
        {
            var collection = new List<DeployScriptParameters>();
            foreach(var p in parameters)
            {
                collection.Add(new DeployScriptParameters {Name = p.name, Value = p.value});
            }
            return collection;
        }

        public string Output { get; private set; }
        public bool ErrorOccurred { get; private set; }
        public string ScriptRun { get; private set; }
    }
}
