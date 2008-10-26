using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Build.Client;

namespace Tests.TfsDeployer.PowerShellRunnerTests
{
    class StubBuildDetail : IBuildDetail
    {
        IBuildAgent IBuildDetail.BuildAgent
        {
            get { throw new NotImplementedException(); }
        }

        Uri IBuildDetail.BuildAgentUri
        {
            get { throw new NotImplementedException(); }
        }

        IBuildDefinition IBuildDetail.BuildDefinition
        {
            get { throw new NotImplementedException(); }
        }

        Uri IBuildDetail.BuildDefinitionUri
        {
            get { throw new NotImplementedException(); }
        }

        bool IBuildDetail.BuildFinished
        {
            get { throw new NotImplementedException(); }
        }

        string IBuildDetail.BuildNumber
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        IBuildServer IBuildDetail.BuildServer
        {
            get { throw new NotImplementedException(); }
        }

        string IBuildDetail.CommandLineArguments
        {
            get { throw new NotImplementedException(); }
        }

        BuildPhaseStatus IBuildDetail.CompilationStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string IBuildDetail.ConfigurationFolderPath
        {
            get { throw new NotImplementedException(); }
        }

        Uri IBuildDetail.ConfigurationFolderUri
        {
            get { throw new NotImplementedException(); }
        }

        void IBuildDetail.Connect()
        {
            throw new NotImplementedException();
        }

        void IBuildDetail.Connect(int pollingInterval, System.ComponentModel.ISynchronizeInvoke synchronizingObject)
        {
            throw new NotImplementedException();
        }

        IBuildDeletionResult IBuildDetail.Delete()
        {
            throw new NotImplementedException();
        }

        void IBuildDetail.Disconnect()
        {
            throw new NotImplementedException();
        }

        string IBuildDetail.DropLocation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        DateTime IBuildDetail.FinishTime
        {
            get { throw new NotImplementedException(); }
        }

        IBuildInformation IBuildDetail.Information
        {
            get { throw new NotImplementedException(); }
        }

        bool IBuildDetail.KeepForever
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string IBuildDetail.LabelName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string IBuildDetail.LastChangedBy
        {
            get { throw new NotImplementedException(); }
        }

        DateTime IBuildDetail.LastChangedOn
        {
            get { throw new NotImplementedException(); }
        }

        string IBuildDetail.LogLocation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        event PollingCompletedEventHandler IBuildDetail.PollingCompleted
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        string IBuildDetail.Quality
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void IBuildDetail.Refresh(string[] informationTypes, QueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        void IBuildDetail.RefreshAllDetails()
        {
            throw new NotImplementedException();
        }

        void IBuildDetail.RefreshMinimalDetails()
        {
            throw new NotImplementedException();
        }

        string IBuildDetail.RequestedBy
        {
            get { throw new NotImplementedException(); }
        }

        string IBuildDetail.RequestedFor
        {
            get { throw new NotImplementedException(); }
        }

        void IBuildDetail.Save()
        {
            throw new NotImplementedException();
        }

        string IBuildDetail.SourceGetVersion
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        DateTime IBuildDetail.StartTime
        {
            get { throw new NotImplementedException(); }
        }

        BuildStatus IBuildDetail.Status
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        event StatusChangedEventHandler IBuildDetail.StatusChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event StatusChangedEventHandler IBuildDetail.StatusChanging
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        void IBuildDetail.Stop()
        {
            throw new NotImplementedException();
        }

        BuildPhaseStatus IBuildDetail.TestStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Uri IBuildDetail.Uri
        {
            get { throw new NotImplementedException(); }
        }

        void IBuildDetail.Wait()
        {
            throw new NotImplementedException();
        }

        #region IBuildDetail Members


        public BuildReason Reason
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
