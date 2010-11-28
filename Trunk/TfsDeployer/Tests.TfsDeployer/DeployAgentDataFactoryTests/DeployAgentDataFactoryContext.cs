using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

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

        public BuildDetail CreateBuildDetail()
        {
            var buildDetail = new BuildDetail {BuildNumber = "test_build_number"};
            return buildDetail;
        }

    }
}