using TfsDeployer.Web.Models;
using TfsDeployer.Web.Presenters;
using WebFormsMvp;
using WebFormsMvp.Web;

namespace TfsDeployer.Web.Views
{
    [PresenterBinding(typeof(ConfigurationPresenter))]
    public partial class ConfigurationView : MvpUserControl<ConfigurationModel>
    {
    }
}