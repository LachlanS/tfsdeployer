using System;

namespace TfsDeployer
{
    public class ConsoleEntryPoint : IProgramEntryPoint
    {
        private readonly TfsDeployerApplication _tfsDeployerApplication;

        public ConsoleEntryPoint(TfsDeployerApplication tfsDeployerApplication)
        {
            _tfsDeployerApplication = tfsDeployerApplication;
        }

        public void Run()
        {
            using (_tfsDeployerApplication)
            {
                try
                {
                    _tfsDeployerApplication.Start();
                    Console.WriteLine("Hit Enter to stop the service");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}