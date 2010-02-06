using Microsoft.TeamFoundation.Client;

namespace TfsDeployer
{
    public interface ITeamFoundationServerProvider
    {
        TeamFoundationServer GetServer();
    }
}