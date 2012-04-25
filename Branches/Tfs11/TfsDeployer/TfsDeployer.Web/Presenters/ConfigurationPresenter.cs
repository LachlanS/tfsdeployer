using System;
using TfsDeployer.Web.Models;
using TfsDeployer.Web.Services;
using WebFormsMvp;

namespace TfsDeployer.Web.Presenters
{
    public class ConfigurationPresenter : Presenter<IView<ConfigurationModel>>
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationPresenter(IView<ConfigurationModel> view) :
            this(view, new ConfigurationService())
        {
        }

        public ConfigurationPresenter(IView<ConfigurationModel> view, IConfigurationService configurationService) : base(view)
        {
            _configurationService = configurationService;

            View.Load += ViewLoad;
        }

        public override void ReleaseView()
        {
            View.Load -= ViewLoad;
        }

        private void ViewLoad(object sender, EventArgs e)
        {
            View.Model.DeployerServiceUrl = _configurationService.GetDeployerInstanceAddress()[0];

            if (!string.IsNullOrEmpty(Request.Form["SaveButton"]))
            {
                var deployerServiceUrl = Request.Form["DeployerServiceUrl"];
                // TODO validate

                _configurationService.SetDeployerInstanceAddress(deployerServiceUrl);

                View.Model.DeployerServiceUrl = deployerServiceUrl;
            } 

        }

    }
}