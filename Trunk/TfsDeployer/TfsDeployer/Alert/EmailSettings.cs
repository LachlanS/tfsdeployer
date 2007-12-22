using System;
using System.Collections.Generic;
using System.Text;

namespace TfsDeployer.Alert
{
    [Serializable]
    public class EmailSettings
    {
        public string From;
        public string To;
        public string SmtpAddress;
    }
}
