using System;
using System.IO;
using System.Management.Automation.Host;
using System.Text;

namespace TfsDeployer.DeployAgent
{
    // Based on Microsoft.Windows.PowerShell.Gui.Internal.HostTextWriter, Microsoft.PowerShell.GPowerShell, Version=1.0.0.0
    public class DeploymentHostTextWriter : TextWriter
    {
        private static readonly TextWriter _originalOut;
        private static readonly TextWriter _originalError;
        private static readonly TextWriter _newOut;
        private static readonly TextWriter _newError;

        [ThreadStatic] private static PSHostUserInterface _threadHostUserInterface;

        static DeploymentHostTextWriter()
        {
            _originalOut = Console.Out;
            _originalError = Console.Error;
            _newOut = new DeploymentHostTextWriter(_originalOut);
            _newError = new DeploymentHostTextWriter(_originalError);
        }

        public static void RegisterHostUserInterface(PSHostUserInterface hostUserInterface)
        {
            Console.SetOut(_newOut);
            Console.SetError(_newError);
            _threadHostUserInterface = hostUserInterface;
        }

        private readonly TextWriter _originalWriter;

        public DeploymentHostTextWriter(TextWriter originalWriter)
        {
            _originalWriter = originalWriter;
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
