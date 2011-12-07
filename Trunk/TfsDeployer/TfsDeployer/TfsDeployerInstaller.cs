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
using System.Collections.Generic;
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
        }

        public static void Install(IEnumerable<string> commandLine)
        {
            InstallAssemblyInTransaction(i => i.Install(new Hashtable()), commandLine);
        }

        public static void Uninstall()
        {
            InstallAssemblyInTransaction(i => i.Uninstall(null  /* requires null */), new string[0]);
        }

        private static void InstallAssemblyInTransaction(Action<Installer> installerAction, IEnumerable<string> commandLine)
        {
            using (var installer = new TfsDeployerInstaller())
            using (var ti = new TransactedInstaller())
            {
                var assemblyPath = String.Format("/assemblypath={0}", installer.GetType().Assembly.Location);
                var commandLineWithAssemblyPath = (new[] {assemblyPath}).Concat(commandLine).ToArray();
                var context = new InstallContext(null, commandLineWithAssemblyPath);

                ti.Installers.Add(installer);
                ti.Context = context;
                installerAction(ti);
            }
        }
    }
}
