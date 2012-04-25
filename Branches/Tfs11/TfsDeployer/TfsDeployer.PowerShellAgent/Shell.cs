using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using Microsoft.PowerShell;

namespace TfsDeployer.PowerShellAgent
{
    public class Shell
    {
        public int Run(AgentRequest request)
        {
            var arguments = BuildPowerShellArguments(request);

            RunspaceConfiguration configuation;
            if (String.IsNullOrEmpty(request.PSConsoleFile))
            {
                configuation = RunspaceConfiguration.Create();
            }
            else
            {
                configuation = RunspaceConfiguration.Create(request.PSConsoleFile);
            }

            return ConsoleShell.Start(configuation, null, null, arguments);
        }

        private static string[] BuildPowerShellArguments(AgentRequest request)
        {
            var arguments = new List<string>();
            arguments.Add("-nologo");
            arguments.Add("-noninteractive");
            arguments.AddRange(new[] { "-inputformat", "none" });

            if (request.NoProfile)
            {
                arguments.Add("-noprofile");
            }

            if (!String.IsNullOrEmpty(request.ExecutionPolicy))
            {
                arguments.AddRange(new[] { "-executionpolicy", request.ExecutionPolicy });
            }

            if (request.ApartmentState == ApartmentState.STA)
            {
                arguments.Add("-sta");
            }
            else if (request.ApartmentState == ApartmentState.MTA)
            {
                arguments.Add("-mta");
            }

            arguments.AddRange(new[] { "-command", request.Command });

            return arguments.ToArray();
        }
    }
}