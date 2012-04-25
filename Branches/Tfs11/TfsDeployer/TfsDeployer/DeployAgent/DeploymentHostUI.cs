using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace TfsDeployer.DeployAgent
{
    public class DeploymentHostUI : PSHostUserInterface
    {
        private static readonly TraceSwitch TraceSwitch = new TraceSwitch("TfsDeployer.DeployAgent.DeploymentHostUI", null, TraceLevel.Warning.ToString());
        private static readonly string ClassName = typeof (DeploymentHostUI).FullName;

        private static void TraceVerbose(string format, params object[] args)
        {
            if (!TraceSwitch.TraceVerbose) return;
            Trace.TraceInformation(format, args);
        }

        private readonly StringBuilder _log = new StringBuilder();
        private readonly DeploymentHostRawUi _rawUi = new DeploymentHostRawUi();
        private bool _hasErrors;

        public string Output
        {
            get { return _log.ToString(); }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            TraceVerbose("{0}.Prompt(\"{1}\", \"{2}\", {3})", ClassName, caption, message, descriptions);
            throw new PSInvalidOperationException();
        }

        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            TraceVerbose("{0}.PromptForChoice(\"{1}\", \"{2}\", {3}, {4})", ClassName, caption, message, choices, defaultChoice);
            throw new PSInvalidOperationException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            TraceVerbose("{0}.PromptForCredential(\"{1}\", \"{2}\", \"{3}\", \"{4}\", {5}, {6})", ClassName, caption, message, userName, targetName, allowedCredentialTypes, options);
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            TraceVerbose("{0}.PromptForCredential(\"{1}\", \"{2}\", \"{3}\", \"{4}\")", ClassName, caption, message, userName, targetName);
            throw new NotImplementedException();
        }

        public override PSHostRawUserInterface RawUI
        {
            get
            {
                TraceVerbose("{0}.RawUI", ClassName);
                return _rawUi;
            }
        }

        public override string ReadLine()
        {
            TraceVerbose("{0}.ReadLine()", ClassName);
            throw new PSInvalidOperationException();
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            TraceVerbose("{0}.ReadLineAsSecureString()", ClassName);
            throw new PSInvalidOperationException();
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            TraceVerbose("{0}.Write({1}, {2}, \"{3}\")", ClassName, foregroundColor, backgroundColor, value);
            // colors are ignored
            Write(value);
        }

        public override void Write(string value)
        {
            TraceVerbose("{0}.Write(\"{1}\")", ClassName, value);
            _log.Append(value);
        }

        public override void WriteDebugLine(string message)
        {
            TraceVerbose("{0}.WriteDebugLine(\"{1}\")", ClassName, message);
            WritePrefixLine(message, "DEBUG");
        }

        public override void WriteErrorLine(string value)
        {
            TraceVerbose("{0}.WriteErrorLine(\"{1}\")", ClassName, value);
            _hasErrors = true;
            WritePrefixLine(value, "ERROR");
        }

        private void WritePrefixLine(string value, string prefix)
        {
            WriteLine(prefix + ": " + value);
        }

        public override void WriteLine(string value)
        {
            TraceVerbose("{0}.WriteLine(\"{1}\")", ClassName, value);
            _log.AppendLine(value);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            TraceVerbose("{0}.WriteProgress({1}, {2})", ClassName, sourceId, record);
            // ignored
        }

        public override void WriteVerboseLine(string message)
        {
            TraceVerbose("{0}.WriteVerboseLine(\"{1}\")", ClassName, message);
            WritePrefixLine(message, "VERBOSE");
        }

        public override void WriteWarningLine(string message)
        {
            TraceVerbose("{0}.WriteWarningLine(\"{1}\")", ClassName, message);
            WritePrefixLine(message, "WARNING");
        }
    }
}