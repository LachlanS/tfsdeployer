using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace TfsDeployer.Agent
{
    [RunInstaller(true)]
    public class AgentInstaller : Installer
    {
        public AgentInstaller()
        {
            var agentService = new AgentWindowsService();

            var agentServiceInstaller = new ServiceInstaller();
            agentServiceInstaller.CopyFromComponent(agentService);
            agentServiceInstaller.StartType = ServiceStartMode.Automatic;
            Installers.Add(agentServiceInstaller);

            var serviceProcessInstaller = new ServiceProcessInstaller();
            Installers.Add(serviceProcessInstaller);
        }
    }
}
