using System;
using Microsoft.TeamFoundation.Build.Client;

namespace TfsDeployer.TeamFoundation
{
    public class BuildDetailToBuildDataAdapter : IBuildData
    {
        private readonly IBuildDetail _detail;

        public BuildDetailToBuildDataAdapter(IBuildDetail buildDetail)
        {
            if (null == buildDetail) throw new ArgumentNullException("buildDetail");
            _detail = buildDetail;
        }
        
        public string BuildMachine
        {
            get { return _detail.BuildController.Name; }
        }

        public string BuildNumber
        {
            get { return _detail.BuildNumber; }
        }

        public string BuildQuality
        {
            get { return _detail.Quality; }
        }

        public string BuildStatus
        {
            get { return _detail.Status.ToString(); }
        }

        public int BuildStatusId
        {
            get { return (int)_detail.Status; }
        }

        public string BuildType
        {
            get { return _detail.BuildDefinition.Name; }
        }

        public string BuildTypeFileUri
        {
            get { return _detail.BuildDefinitionUri.ToString(); }
        }

        public string BuildUri
        {
            get { return _detail.Uri.ToString(); }
        }

        public string DropLocation
        {
            get { return _detail.DropLocation; }
        }

        public DateTime FinishTime
        {
            get { return _detail.FinishTime; }
        }

        public bool GoodBuild
        {
            get
            {
                return _detail.Status == Microsoft.TeamFoundation.Build.Client.BuildStatus.Succeeded;
            }
        }

        public string LastChangedBy
        {
            get { return _detail.LastChangedBy; }
        }

        public DateTime LastChangedOn
        {
            get { return _detail.LastChangedOn; }
        }

        public string LogLocation
        {
            get { return _detail.LogLocation; }
        }

        public string RequestedBy
        {
            get { return _detail.RequestedBy; }
        }

        public DateTime StartTime
        {
            get { return _detail.StartTime; }
        }

        public string TeamProject
        {
            get { return _detail.BuildDefinition.TeamProject; }
        }
    }
}