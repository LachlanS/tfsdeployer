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
using System.Text;
using TfsDeployer.Notifier;
using TfsDeployer.Runner;
using System.Net.Mail;
using TfsDeployer.Properties;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer.Alert
{
    public class EmailAlerter : IAlert
    {
        public void Alert(Mapping mapping, IBuildData build, IRunner runner)
        {
            try
            {
                var client = new SmtpClient(Settings.Default.SmtpServer);
                var subject = GetSubject(mapping, build, runner);
                var body = GetBody(mapping, build, runner);
                var toAddress = mapping.NotificationAddress ?? Settings.Default.ToAddress;
                
                var message = new MailMessage
                                  {
                                      From = new MailAddress(Settings.Default.FromAddress),
                                      Subject = subject,
                                      Body = body
                                  };

                // Allow multiple recipients separated by semi-colon
                foreach (var address in toAddress.Split(';'))
                {
                    message.To.Add(address);
                }

                client.Send(message);
            }
            catch(Exception ex)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, ex);
            }
        }

        private static string GetBody(Mapping map, IBuildData build, IRunner runner)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("Team Project/Build: {0} to {1}",build.TeamProject,build.BuildType));
            builder.AppendLine(string.Format("Quality Change: {0} to {1}",map.OriginalQuality,map.NewQuality));
            builder.AppendLine(string.Format("Drop Location: {0}", build.DropLocation));
            builder.AppendLine(string.Format("Build Uri: {0}", build.BuildUri));
            builder.AppendLine(string.Format("Script: {0}", runner.ScriptRun));
            builder.AppendLine(string.Format("Executed on Machine: {0}", map.Computer));
            builder.AppendLine("Output:");
            builder.AppendLine(runner.Output);
            return builder.ToString();
        }

        private static string GetSubject(Mapping map, IBuildData build, IRunner runner)
        {
            var errorMessage = "Success: ";
            if (runner.ErrorOccurred)
            {
                errorMessage = "Failed: ";
            }
            
            return string.Format("{0} TfsDeployer Ran Script {1} on Machine {2} for {3}/{4}/{5}",errorMessage, map.Script, map.Computer, build.TeamProject, build.BuildType, build.BuildNumber);
        }
    }
}
