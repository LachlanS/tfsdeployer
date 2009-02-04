using System;
using System.Reflection;
using System.ServiceProcess;

namespace TfsDeployer.Agent
{
    public class AgentWindowsService : ServiceBase
    {
        private readonly DeployAgentListener _listener;

        public AgentWindowsService()
        {
            ServiceName = Assembly.GetExecutingAssembly().GetName().Name;
            _listener = new DeployAgentListener();
        }

        protected override void OnStart(string[] args)
        {
            Action startAction = _listener.Start;
            startAction.BeginInvoke(null, null);
        }

        protected override void OnStop()
        {
            _listener.Stop();
        }
    }
}
