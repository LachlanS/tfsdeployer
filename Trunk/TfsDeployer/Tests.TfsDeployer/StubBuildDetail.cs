using System;
using System.ComponentModel;
using Microsoft.TeamFoundation.Build.Client;
using Rhino.Mocks;

namespace Tests.TfsDeployer
{
    internal class StubBuildDetail : IBuildDetail
    {
        private int _saveCount;
        
        public StubBuildDetail()
        {
            BuildController = MockRepository.GenerateStub<IBuildController>();
            BuildDefinition = MockRepository.GenerateStub<IBuildDefinition>();
            BuildDefinitionUri = new Uri("http://builddefinitionuri");
            TeamProject = "StubTeamProject";
            Uri = new Uri("http://uri");
        }

        public int SaveCount { get { return _saveCount; } }

        public void Connect(int pollingInterval, ISynchronizeInvoke synchronizingObject)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public IBuildDeletionResult Delete()
        {
            throw new NotImplementedException();
        }

        public IBuildDeletionResult Delete(DeleteOptions options)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void FinalizeStatus()
        {
            throw new NotImplementedException();
        }

        public void FinalizeStatus(BuildStatus status)
        {
            throw new NotImplementedException();
        }

        public void RefreshMinimalDetails()
        {
            throw new NotImplementedException();
        }

        public void RefreshAllDetails()
        {
            throw new NotImplementedException();
        }

        public void Refresh(string[] informationTypes, QueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _saveCount++;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Wait()
        {
            throw new NotImplementedException();
        }

        public string BuildNumber { get; set; }
        public BuildPhaseStatus CompilationStatus { get; set; }
        public string ConfigurationFolderPath { get; set; }
        public string DropLocation { get; set; }
        public string DropLocationRoot { get; set; }
        public string LabelName { get; set; }
        public bool KeepForever { get; set; }
        public string LogLocation { get; set; }
        public string Quality { get; set; }
        public BuildStatus Status { get; set; }
        public BuildPhaseStatus TestStatus { get; set; }
        public IBuildAgent BuildAgent { get; set; }
        public Uri BuildAgentUri { get; set; }
        public IBuildController BuildController { get; set; }
        public Uri BuildControllerUri { get; set; }
        public IBuildDefinition BuildDefinition { get; set; }
        public Uri BuildDefinitionUri { get; set; }
        public bool BuildFinished { get; set; }
        public IBuildServer BuildServer { get; set; }
        public string CommandLineArguments { get; set; }
        public IBuildInformation Information { get; set; }
        public Uri ConfigurationFolderUri { get; set; }
        public string LastChangedBy { get; set; }
        public DateTime LastChangedOn { get; set; }
        public string ProcessParameters { get; set; }
        public BuildReason Reason { get; set; }
        public string RequestedBy { get; set; }
        public string RequestedFor { get; set; }
        public string ShelvesetName { get; set; }
        public bool IsDeleted { get; set; }
        public string SourceGetVersion { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public Uri Uri { get; set; }
        public string TeamProject { get; set; }

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

        event PollingCompletedEventHandler IBuildDetail.PollingCompleted
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }

}
