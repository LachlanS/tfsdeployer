using System;
using TfsDeployer.Web.Models;
using TfsDeployer.Web.Services;
using WebFormsMvp;

namespace TfsDeployer.Web.Presenters
{
    public class UptimePresenter : Presenter<IView<UptimeModel>>
    {
        private readonly IDataService _dataService;
        private readonly IConfigurationService _configurationService;

        public UptimePresenter(IView<UptimeModel> view) : 
            this(view, new DataService(new ConfigurationService()), new ConfigurationService())
        {
        }

        public UptimePresenter(IView<UptimeModel> view, IDataService dataService, IConfigurationService configurationService) : base(view)
        {
            _dataService = dataService;
            _configurationService = configurationService;

            view.Load += ViewLoad;
        }

        public override void ReleaseView()
        {
            View.Load -= ViewLoad;
        }

        void ViewLoad(object sender, EventArgs e)
        {
            View.Model.DeployerInstanceName = _configurationService.GetDeployerInstanceAddress()[0];
            View.Model.UptimeText = _dataService.GetUptime().ToString();
        }

    }
}