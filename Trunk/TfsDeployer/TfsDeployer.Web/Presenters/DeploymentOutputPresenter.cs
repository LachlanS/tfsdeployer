using System;
using System.Web;
using TfsDeployer.Web.Models;
using TfsDeployer.Web.Services;
using WebFormsMvp;

namespace TfsDeployer.Web.Presenters
{
    public class DeploymentOutputPresenter : Presenter<IView<DeploymentOutputModel>>
    {
        private readonly IConfigurationService _configurationService;

        public DeploymentOutputPresenter(IView<DeploymentOutputModel> view) :
            this(view, new ConfigurationService())
        { }
        
        public DeploymentOutputPresenter(IView<DeploymentOutputModel> view, IConfigurationService configurationService) : base(view)
        {
            _configurationService = configurationService;
            view.Load += ViewLoad;
        }

        public override void ReleaseView()
        {
            View.Load -= ViewLoad;
        }

        void ViewLoad(object sender, EventArgs e)
        {
            int deploymentId;
            if (!int.TryParse(HttpContext.Request.QueryString["deploymentid"], out deploymentId))
            {
                View.Model.HtmlEncodedOutput = "Invalid deployment id.";
                return;
            }

            var deploymentOutput = _configurationService.CreateDeployerService(0).GetDeploymentOutput(deploymentId);
            View.Model.HtmlEncodedOutput = HttpUtility.HtmlEncode(deploymentOutput.Content);
            View.Model.IsFinal = deploymentOutput.IsFinal;
        }

    }
}