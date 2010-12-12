using WebShell.Hosting;

namespace TfsDeployer.Hosting
{
    public class DeployerContextProvider : HostContextProvider
    {
        public override object GetHostContext()
        {
            return new DeployerContext();
        }
    }
}