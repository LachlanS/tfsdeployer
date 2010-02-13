using Microsoft.TeamFoundation.Client;

namespace TfsDeployer
{
    public interface ITfsConnectionProvider
    {
        TfsConnection GetConnection();
    }
}