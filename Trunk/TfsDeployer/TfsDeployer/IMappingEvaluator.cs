using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;

namespace TfsDeployer
{
    public interface IMappingEvaluator
    {
        bool DoesMappingApply(Mapping mapping, BuildStatusChangeEvent triggerEvent, string buildStatus);
    }
}