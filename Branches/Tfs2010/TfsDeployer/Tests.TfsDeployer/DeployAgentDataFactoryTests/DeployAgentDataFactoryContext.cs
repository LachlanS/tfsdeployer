using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer;
using TfsDeployer.Configuration;

namespace Tests.TfsDeployer.DeployAgentDataFactoryTests
{
    public class DeployAgentDataFactoryContext
    {
        public const string DeployScriptRoot = @"c:\deploy_script_root\";
        
        public Mapping CreateMapping()
        {
            return new Mapping
                       {
                           NewQuality = "new_quality",
                           OriginalQuality = "original_quality",
                           Computer = "deploy_server",
                           Script = "deploy_script_file",
                           ScriptParameters = new[]
                                                  {
                                                      new ScriptParameter
                                                          {
                                                              Name = "first_parameter_name",
                                                              Value = "first_parameter_value"
                                                          },
                                                      new ScriptParameter
                                                          {
                                                              Name = "second_parameter_name",
                                                              Value = "second_parameter_value"
                                                          }
                                                  }
                       };
        }

        public BuildInformation CreateBuildInformation()
        {
            IBuildDetail buildDetail = new StubBuildDetail();
            buildDetail.BuildNumber = "test_build_number";
            return new BuildInformation(buildDetail);
        }

    }
}