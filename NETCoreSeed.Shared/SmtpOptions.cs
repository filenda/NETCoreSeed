using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Shared
{
    public class SmtpOptions
    {
        public string Server { get; set; } = "smtpout.secureserver.net";
        public int Port { get { return 465; } }
        //public int Port { get { return 25; } }
        //public int Port { get { return 587; } }
        public string User { get; set; } = "xx@xx.com";
        public string Password { get; set; } = "xxxx";
        public bool UseSsl { get; set; } = true;
        //public bool UseSsl { get; set; } = false;
        public bool RequiresAuthentication { get; set; } = true;
        public string PreferredEncoding { get; set; } = string.Empty;
    }
}