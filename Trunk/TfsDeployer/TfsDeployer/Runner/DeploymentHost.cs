using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation.Host;
using System.Threading;
using System.Globalization;
using System.Reflection;

namespace TfsDeployer.Runner
{
    public class DeploymentHost : PSHost
    {
        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        public override void EnterNestedPrompt()
        {
        }

        public override void ExitNestedPrompt()
        {
        }

        private Guid m_InstanceId = Guid.Empty;

        public override Guid InstanceId
        {
            get
            {
                if (this.m_InstanceId == Guid.Empty)
                {
                    this.m_InstanceId = Guid.NewGuid();
                }

                return this.m_InstanceId;
            }
        }

        public override string Name
        {
            get { return typeof(DeploymentHost).Name;  }
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override void SetShouldExit(int exitCode)
        {
        }

        public override PSHostUserInterface UI
        {
            get { return null; }
        }

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
    }
}
