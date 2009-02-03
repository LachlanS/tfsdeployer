using System;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class BuildInformation
    {
        private IBuildData _data;
        private BuildDetail _detail;

        public BuildInformation(BuildDetail buildDetail)
        {
            if (null == buildDetail) throw new ArgumentNullException("buildDetail");
            _detail = buildDetail;
            _data = new BuildDetailToBuildDataAdapter(buildDetail);
        }

        public IBuildData Data
        {
            get { return _data; }
        }

        public BuildDetail Detail
        {
            get { return _detail; }
        }

    }
}
