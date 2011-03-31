using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// This method does what it says on the tin. BE VERY CAREFUL when using it - it will recursively attempt
        /// to kill the given process and any processes created by it.
        /// </summary>
        /// <param name="proc"></param>
        public static void KillRecursive(this Process proc)
        {
            var childProcesses = Process.GetProcesses()
                .Where(p => p.GetParentId() == proc.Id)
                .ToList();

            foreach (var childProcess in childProcesses)
            {
                childProcess.KillRecursive();
            }

            try
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Killing process {0}", proc.ProcessName);
                proc.Kill();
            }
            catch (Exception exc)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, "Failed to kill process {0}: {1}", proc.ProcessName, exc.Message);
            }
        }

        public static Process GetParent(this Process process)
        {
            int ppid = process.GetParentId();
            if (ppid == 0) return null;

            return Process.GetProcessById(ppid);
        }

        public static int GetParentId(this Process process)
        {
            int pid = process.Id;
            int ppid = 0;
            try
            {
                using (ManagementObject mgmtObj = new ManagementObject("win32_process.handle='" + pid.ToString() + "'"))
                {
                    mgmtObj.Get();
                    ppid = Convert.ToInt32(mgmtObj["ParentProcessId"]);
                }
            }
            catch (ManagementException exc)
            {
                // processes have a habit of vanishing just when we're trying to find them. That's okay - 0
                // is already a magic number.  /sigh  -andrewh 27/10/2010
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Could not find process with id {0}: {1}", pid, exc.Message);
            }

            return ppid;
        }
    }
}
