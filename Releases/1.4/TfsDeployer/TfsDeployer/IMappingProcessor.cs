using System.Collections.Generic;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public interface IMappingProcessor
    {
        void ProcessMappings(IEnumerable<Mapping> mappings, BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, IPostDeployAction postDeployAction);
    }
}