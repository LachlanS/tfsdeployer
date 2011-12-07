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

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace TfsDeployer
{
    [RunInstaller(true)]
    public class TfsDeployerInstaller : Installer
    {
        public TfsDeployerInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            Installers.Add(processInstaller);

            var deployerServiceInstaller = new ServiceInstaller
                                               {
                                                   DisplayName = "TFS Deployer",
                                                   ServiceName = "TfsDeployer",
                                                   Description = "Performs deployment for Team Foundation Server builds.",
                                                   StartType = ServiceStartMode.Automatic,
                                                   ServicesDependedOn = new[] {"HTTP"}
                                               };
            processInstaller.Installers.Add(deployerServiceInstaller);

            SetServiceAccount(null, null);
        }

        private ServiceProcessInstaller ProcessInstaller
        {
            get { return Installers.OfType<ServiceProcessInstaller>().Single(); }
        }

        public void SetServiceAccount(string username, string password)
        {
            var processInstaller = ProcessInstaller;
            if (String.IsNullOrEmpty(username))
            {
                processInstaller.Account = ServiceAccount.LocalService;
            }
            else
            {
                processInstaller.Account = ServiceAccount.User;
                processInstaller.Username = username;
                processInstaller.Password = password;
            }
        }

        public static void Install(string username, string password)
        {
            using (var deployerInstaller = new TfsDeployerInstaller())
            {
                deployerInstaller.SetServiceAccount(username, password);
                InstallAssemblyInTransaction(deployerInstaller, i => i.Install(new Hashtable()));
            }
        }

        public static void Uninstall()
        {
            using (var deployerInstaller = new TfsDeployerInstaller())
                InstallAssemblyInTransaction(deployerInstaller, i => i.Uninstall(null  /* requires null */));
        }

        private static void InstallAssemblyInTransaction(Installer installer, Action<Installer> installerAction)
        {
            var path = String.Format("/assemblypath={0}", installer.GetType().Assembly.Location);
            var context = new InstallContext("", new[] { path });

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(installer);
                ti.Context = context;
                installerAction(ti);
            }
        }
    }
}
