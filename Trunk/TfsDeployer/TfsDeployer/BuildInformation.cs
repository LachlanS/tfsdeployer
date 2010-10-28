using System;
using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer.TeamFoundation;

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

        public IBuildData Data
        {
            get
            {
                return new BuildData
                           {
                               BuildMachine = _detail.BuildController.Name,
                               BuildNumber = _detail.BuildNumber,
                               BuildQuality = _detail.Quality,
                               BuildStatus = _detail.Status.ToString(),
                               BuildStatusId = (int)_detail.Status,
                               BuildType = _detail.BuildDefinition.Name,
                               BuildTypeFileUri = _detail.BuildDefinitionUri.ToString(),
                               BuildUri = _detail.Uri.ToString(),
                               DropLocation = _detail.DropLocation,
                               FinishTime = _detail.FinishTime,
                               GoodBuild = _detail.Status == BuildStatus.Succeeded,
                               LastChangedBy = _detail.LastChangedBy,
                               LastChangedOn = _detail.LastChangedOn,
                               LogLocation = _detail.LogLocation,
                               RequestedBy = _detail.RequestedBy,
                               StartTime = _detail.StartTime,
                               TeamProject = _detail.TeamProject
                           };
            }
        }

        public IBuildDetail Detail
        {
            get { return _detail; }
        }

    }
}
