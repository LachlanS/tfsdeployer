using System.Threading;

namespace TfsDeployer.PowerShellAgent
{
    public class AgentRequest
    {
        public AgentRequest()
        {
            ApartmentState = ApartmentState.Unknown;
        }

        public string PSConsoleFile { get; set; }
        public bool NoProfile { get; set; }
        public string ExecutionPolicy { get; set; }
        public ApartmentState ApartmentState { get; set; }
        public string Command { get; set; }
        
    }
}