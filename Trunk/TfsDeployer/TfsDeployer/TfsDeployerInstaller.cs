      using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
namespace TfsDeployer
{
      [RunInstaller(true)]
    public class TfsDeployerInstaller : Installer
    {
        private EventLogInstaller _eventLogInstaller;
        private ServiceInstaller _tfsIntegratorInstaller;
        private ServiceProcessInstaller _processInstaller;
        public TfsDeployerInstaller()
        {

            _processInstaller = new ServiceProcessInstaller();
            this._eventLogInstaller = new System.Diagnostics.EventLogInstaller();
            this._tfsIntegratorInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // _eventLogInstaller
            // 
            this._eventLogInstaller.CategoryCount = 0;
            this._eventLogInstaller.CategoryResourceFile = null;
            this._eventLogInstaller.Log = "Application";
            this._eventLogInstaller.MessageResourceFile = null;
            this._eventLogInstaller.ParameterResourceFile = null;
            this._eventLogInstaller.Source = "Readify.TfsDeployer";
            // 
            // _tfsIntegratorInstaller
            // 
            this._tfsIntegratorInstaller.DisplayName = "TFS Deployer";
            this._tfsIntegratorInstaller.ServiceName = "TfsDeployer";
            this._tfsIntegratorInstaller.Description = "Performs Deployment for Team Foundation Server";
            this._tfsIntegratorInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            _processInstaller.Account = ServiceAccount.LocalSystem; 
            this.Installers.Add(_eventLogInstaller);
            this.Installers.Add(_tfsIntegratorInstaller);
            this.Installers.Add(_processInstaller);
        }
    }
}
