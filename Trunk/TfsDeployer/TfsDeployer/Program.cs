using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ServiceProcess;
using System.Configuration.Install;
using Readify.Useful.TeamFoundation.Common.Listener;
using System.Collections;
using Genghis;
using Readify.Useful.TeamFoundation.Common.Notification;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer
{
    public class Program : ServiceBase
    {
        TfsListener listener = new TfsListener();
        Deployer deployer = new Deployer();
        public static void Main(string[] args)
        {

            Program controller = new Program();
            if (args.Length > 0)
            {
                if (args[0] == "-i")
                {
                    Install();
                }
                else if (args[0] == "-u")
                {
                    Uninstall();
                }
                else if (args[0] == "-d")
                {
                    try
                    {
                        //Start the service
                        TextWriterTraceListener listener = new TextWriterTraceListener(Console.Out);
                        Trace.Listeners.Add(listener);
                        controller.OnStart(new string[] { });
                        Console.WriteLine("HIt Enter to stop the service");
                        Console.ReadKey();
                        controller.Stop();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }
                else
                {


                    CommandLine command = new CommandLine();
                    if (command.ParseAndContinue(args))
                    {
                        try
                        {
                            Encrypter.Encryt(command);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(command.GetUsage());
                            Console.WriteLine(ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine(command.GetUsage());
                    }


                }

            }
            else
            {
                System.ServiceProcess.ServiceBase[] ServicesToRun;
                ServicesToRun = new System.ServiceProcess.ServiceBase[] { controller };
                System.ServiceProcess.ServiceBase.Run(ServicesToRun);
            }
        }

        protected override void OnStart(string[] args)
        {
            listener.BuildStatusChangeEventReceived += new EventHandler<BuildStatusChangeEventArgs>(listener_BuildStatusChangeEventReceived);
            listener.Start();
        }

        protected override void OnStop()
        {
            listener.Stop();
        }

        private delegate void ExecuteDeploymentProcessDelegate(BuildStatusChangeEvent ev);

        void listener_BuildStatusChangeEventReceived(object sender, BuildStatusChangeEventArgs e)
        {
            ExecuteDeploymentProcessDelegate edpd = new ExecuteDeploymentProcessDelegate(deployer.ExecuteDeploymentProcess);
            edpd.BeginInvoke(e.EventRaised, null, null);
        }

        private static void Install()
        {
            TransactedInstaller ti = new TransactedInstaller();
            TfsDeployerInstaller mi = new TfsDeployerInstaller();
            ti.Installers.Add(mi);
            String path = String.Format("/assemblypath={0}",
              System.Reflection.Assembly.GetExecutingAssembly().Location);
            String[] cmdline = { path };
            InstallContext ctx = new InstallContext("", cmdline);
            ti.Context = ctx;
            ti.Install(new Hashtable());

        }

        private static void Uninstall()
        {
            TransactedInstaller ti = new TransactedInstaller();
            TfsDeployerInstaller mi = new TfsDeployerInstaller();
            ti.Installers.Add(mi);
            String path = String.Format("/assemblypath={0}",
            System.Reflection.Assembly.GetExecutingAssembly().Location);
            String[] cmdline = { path };
            InstallContext ctx = new InstallContext("", cmdline);
            ti.Context = ctx;
            ti.Uninstall(null);

        }
    }
}
