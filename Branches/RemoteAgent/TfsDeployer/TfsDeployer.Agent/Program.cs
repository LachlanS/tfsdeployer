using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace TfsDeployer.Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServiceBase.Run(new AgentWindowsService());
                return;
            }

            var switchKey = args[0];
            if (switchKey.Length == 2)
            {
                switchKey = switchKey.Substring(1);
            }

            var switchActions = SwitchActions();
            if (switchActions.ContainsKey(switchKey))
            {
                switchActions[switchKey].Invoke();
            }
        }

        private static IDictionary<string, Action> SwitchActions()
        {
            var switchActions = new Dictionary<string, Action>(StringComparer.InvariantCultureIgnoreCase)
                                    {
                                        {"i", Install},
                                        {"u", Uninstall},
                                        {"d", Debug}
                                    };
            return switchActions;
        }

        private static void Debug()
        {
            var listener = new DeployAgentListener();
            listener.Start();

            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();

            listener.Stop();
        }

        private static TransactedInstaller GetTransactedInstaller()
        {
            var agentInstaller = new AgentInstaller();
            var commandLine = string.Format("/assemblyPath={0}", Assembly.GetExecutingAssembly().Location);

            var txInstaller = new TransactedInstaller();
            txInstaller.Installers.Add(agentInstaller);
            txInstaller.Context = new InstallContext(null, new[] { commandLine });
            return txInstaller;
        }

        private static void Install()
        {
            var txInstaller = GetTransactedInstaller();
            txInstaller.Install(new Hashtable());
        }

        private static void Uninstall()
        {
            var txInstaller = GetTransactedInstaller();
            txInstaller.Uninstall(new Hashtable());
        }

    }
}
