using TfsDeployer.Runner;

namespace TfsDeployer
{
    public interface IRunnerProvider
    {
        IRunner GetRunner(Mapping map);
    }
}