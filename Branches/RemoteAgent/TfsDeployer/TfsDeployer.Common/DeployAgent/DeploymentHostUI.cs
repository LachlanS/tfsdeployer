using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation.Host;

namespace TfsDeployer.DeployAgent
{
    public class DeploymentHostUI : PSHostUserInterface
    {
        private readonly StringBuilder _log = new StringBuilder();

        public string ReadLog()
        {
            return _log.ToString();
        }

        public override Dictionary<string, System.Management.Automation.PSObject> Prompt(string caption, string message, System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }

        public override System.Management.Automation.PSCredential PromptForCredential(string caption, string message, string userName, string targetName, System.Management.Automation.PSCredentialTypes allowedCredentialTypes, System.Management.Automation.PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override System.Management.Automation.PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return null; /* does not support low-level interaction */ }
        }

        public override string ReadLine()
        {
            throw new NotImplementedException();
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException();
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            // colors are ignored
            Write(value);
        }

        public override void Write(string value)
        {
            _log.Append(value);
        }

        public override void WriteDebugLine(string message)
        {
            WritePrefixLine(message, "DEBUG");
        }

        public override void WriteErrorLine(string value)
        {
            WritePrefixLine(value, "ERROR");
        }

        private void WritePrefixLine(string value, string prefix)
        {
            _log.Append(prefix);
            _log.Append(": ");
            WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            _log.AppendLine(value);
        }

        public override void WriteProgress(long sourceId, System.Management.Automation.ProgressRecord record)
        {
            // ignored
        }

        public override void WriteVerboseLine(string message)
        {
            WritePrefixLine(message, "VERBOSE");
        }

        public override void WriteWarningLine(string message)
        {
            WritePrefixLine(message, "WARNING");
        }
    }
}