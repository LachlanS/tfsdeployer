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
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using Autofac;

namespace TfsDeployer
{
    public static class Program
    {
        private static IContainer _container;

        public static void Main(string[] args)
        {
            var mode = DeployerContainerBuilder.RunMode.WindowsService;

            if (args.Length > 0)
            {
                if (args[0] == "-i")
                {
                    Install();
                    return;
                }
                if (args[0] == "-u")
                {
                    Uninstall();
                    return;
                }
                if (args[0] == "-d")
                {
                    mode = DeployerContainerBuilder.RunMode.InteractiveConsole;
                }
            }

            var containerBuilder = new DeployerContainerBuilder(mode);
            _container = containerBuilder.Build();

            Trace.Listeners.Add(_container.Resolve<TraceListener>());
            _container.Resolve<IProgramEntryPoint>().Run();
        }

        private static void Install()
        {
            RunInstaller(i => i.Install(new Hashtable()));
        }

        private static void Uninstall()
        {
            RunInstaller(i => i.Uninstall(null)); // requires null.
        }

        private static void RunInstaller(Action<Installer> installerAction)
        {
            using (var ti = new TransactedInstaller())
            using (var mi = new TfsDeployerInstaller())
            {
                ti.Installers.Add(mi);
                var path = String.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
                var ctx = new InstallContext("", new[] { path });
                ti.Context = ctx;
                installerAction(ti);
            }
        }

    }
}
