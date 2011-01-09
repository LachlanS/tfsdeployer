using System;
using TfsDeployer.Web.Services;
using TfsDeployer.Web.Views;
using WebFormsMvp;

namespace TfsDeployer.Web.Presenters
{
    public class EventListPresenter : Presenter<EventListView>
    {
        private readonly IDataService _dataService;

        public EventListPresenter(EventListView view) 
            : this(new DataService(new ConfigurationService()), view)
        { }

        public EventListPresenter(IDataService dataService, EventListView view) 
            : base(view)
        {
            _dataService = dataService;
            InitialiseViewSubscriptions();
        }

        public void InitialiseViewSubscriptions()
        {
            View.Load += ViewLoad;
        }

        void ViewLoad(object sender, EventArgs e)
        {
            GetDeploymentEvents(5);
        }

        public override void ReleaseView()
        {
            View.Load -= ViewLoad;
        }

        public void GetDeploymentEvents(int maximumEventCount)
        {
            View.Model.RecentEvents = _dataService.GetRecentEvents(maximumEventCount);
        }
    }
}