using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public interface IMappingEvaluator
    {
        bool DoesMappingApply(Mapping mapping, BuildStatusChangeEvent triggerEvent, string buildStatus);
    }
}