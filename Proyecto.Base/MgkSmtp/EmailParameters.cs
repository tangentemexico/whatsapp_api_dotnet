using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.MgkSmtp
{
    public class EmailParameters
    {
        public string sSmtpHost { get; set; } = "";
        public string sSmtpUser { get; set; } = "";
        public string sSmtpPassword { get; set; } = "";
        public bool bSmtpSsl { get; set; } = false;
        public int iSmtpPort { get; set; } = 0;
        public bool bSmtpStatus { get; set; } = false;

        public String sSmtpFromName { get; set; }
        public String sSmtpFrom { get; set; }

        public String email_sign = "";
        public String owner = "";
    }
}
