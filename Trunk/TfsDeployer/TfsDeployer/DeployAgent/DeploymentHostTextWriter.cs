using System;
using System.IO;
using System.Management.Automation.Host;
using System.Text;

namespace TfsDeployer.DeployAgent
{
    // Based on Microsoft.Windows.PowerShell.Gui.Internal.HostTextWriter, Microsoft.PowerShell.GPowerShell, Version=1.0.0.0
    public class DeploymentHostTextWriter : TextWriter
    {
        private static readonly TextWriter _originalWriter;
        private static readonly TextWriter _newWriter;

        [ThreadStatic] private static PSHostUserInterface _threadHostUserInterface;

        static DeploymentHostTextWriter()
        {
            _originalWriter = Console.Out;
            _newWriter = new DeploymentHostTextWriter();
        }

        public static void RegisterHostUserInterface(PSHostUserInterface hostUserInterface)
        {
            Console.SetOut(_newWriter);
            _threadHostUserInterface = hostUserInterface;
        }

        public override void Write(string value)
        {
            if (_threadHostUserInterface == null)
            {
                _originalWriter.Write(value);
            }
            else
            {
                _threadHostUserInterface.Write(value);
            }
        }

        public override void WriteLine(string value)
        {
            if (_threadHostUserInterface == null)
            {
                _originalWriter.WriteLine(value);
            }
            else
            {
                _threadHostUserInterface.WriteLine(value);
            }
        }

        public override Encoding Encoding
        {
            get { return Console.OutputEncoding; }
        }
    }
}
