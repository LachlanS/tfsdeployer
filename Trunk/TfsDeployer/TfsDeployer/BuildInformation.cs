using Microsoft.TeamFoundation.Build.Client;
using System;
namespace TfsDeployer
{
    public class BuildInformation
    {
        private IBuildData _data;
        private IBuildDetail _detail;

        public BuildInformation(IBuildDetail buildDetail)
        {
            if (null == buildDetail) throw new ArgumentNullException("buildDetail");
            _detail = buildDetail;
            _data = new BuildDetailToBuildDataAdapter(buildDetail);
        }

        public IBuildData Data
        {
            get { return _data; }
        }

        public IBuildDetail Detail
        {
            get { return _detail; }
        }

    }
}
