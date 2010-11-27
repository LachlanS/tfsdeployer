using System;
using Microsoft.TeamFoundation.Build.Client;

namespace TfsDeployer
{
    public class BuildInformation
    {
        private readonly IBuildDetail _detail;

        public BuildInformation(IBuildDetail buildDetail)
        {
            if (null == buildDetail) throw new ArgumentNullException("buildDetail");
            _detail = buildDetail;
        }

        public IBuildDetail Detail
        {
            get { return _detail; }
        }

    }
}
