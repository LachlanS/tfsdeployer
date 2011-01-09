using System;
using System.Web;
using TfsDeployer.Web.Models;
using TfsDeployer.Web.Services;
using WebFormsMvp;

namespace TfsDeployer.Web.Presenters
{
    public class DeploymentOutputPresenter : Presenter<IView<DeploymentOutputModel>>
    {
        private readonly IDataService _dataService;

        public DeploymentOutputPresenter(IView<DeploymentOutputModel> view) :
            this(view, new DataService(new ConfigurationService()))
        { }
        
        public DeploymentOutputPresenter(IView<DeploymentOutputModel> view, IDataService dataService) : base(view)
        {
            _dataService = dataService;
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

            var output = _dataService.GetDeploymentOutput(deploymentId);
            View.Model.HtmlEncodedOutput = HttpUtility.HtmlEncode(output);
        }

    }
}