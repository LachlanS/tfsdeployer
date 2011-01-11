// Copyright (c) 2007 Readify Pty. Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
        private ServiceInstaller _tfsDeployerInstaller;
        private ServiceProcessInstaller _processInstaller;

        public TfsDeployerInstaller()
        {
            _processInstaller = new ServiceProcessInstaller();
            _eventLogInstaller = new EventLogInstaller();
            _tfsDeployerInstaller = new ServiceInstaller();

            _eventLogInstaller.CategoryCount = 0;
            _eventLogInstaller.CategoryResourceFile = null;
            _eventLogInstaller.Log = "Application";
            _eventLogInstaller.MessageResourceFile = null;
            _eventLogInstaller.ParameterResourceFile = null;
            _eventLogInstaller.Source = "TfsDeployer";

            _tfsDeployerInstaller.DisplayName = "TFS Deployer";
            _tfsDeployerInstaller.ServiceName = "TfsDeployer";
            _tfsDeployerInstaller.Description = "Performs deployment for Team Foundation Server builds.";
            _tfsDeployerInstaller.StartType = ServiceStartMode.Automatic;
            _tfsDeployerInstaller.ServicesDependedOn = new[] {"HTTP"};
            _processInstaller.Account = ServiceAccount.LocalSystem; 

            Installers.Add(_eventLogInstaller);
            Installers.Add(_tfsDeployerInstaller);
            Installers.Add(_processInstaller);
        }
    }
}
