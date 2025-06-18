using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.MgkSmtp
{
    public class MgkSmtpModel
    {
        public String Subject { get; set; }
        public String Body { get; set; }
        public bool IsBodyHtml { get; set; } = true;
        public String Encoding { get; set; }

        public String To { get; set; }
        public String Cc { get; set; }
        public String Bcc { get; set; }
        public String AttachPath { get; set; }

        public EmailParameters Parameters { get; set; }

        public List<UsuarioModel> ToList { get; set; }
        public List<UsuarioModel> CcList { get; set; }
        public List<UsuarioModel> BccList { get; set; }
        public List<String> AttachPathList { get; set; }

        public void SetMgkSmtpModel()
        {
            this.SetAttachPath();
            this.SetBcc();
            this.SetCc();
            this.SetTo();
        }

        public void SetAttachPath(string sattachPath = "")
        {
            if (sattachPath != "")
                AttachPath = sattachPath;
            AttachPath = AttachPath ?? "";
            if (AttachPath == "")
                return;
            string[] tmp = AttachPath.Split(',');
            if (AttachPathList == null)
                AttachPathList = new List<String>();
            else
                AttachPathList.Clear();
            foreach (string s in tmp)
                if (s != "")
                    AttachPathList.Add(s);
        }

        public void SetTo(string sto = "")
        {
            if (sto != "")
                To = sto;
            To = To ?? "";
            if (To == "")
                return;
            string[] tmp = To.Split(',');
            if (ToList == null)
                ToList = new List<UsuarioModel>();
            else
                ToList.Clear();
            foreach (string s in tmp)
                if (s != "")
                    ToList.Add(new UsuarioModel { Email = s });
        }

        public void SetCc(string scc = "")
        {
            if (scc != "")
                Cc = scc;
            Cc = Cc ?? "";
            if (Cc == "")
                return;
            string[] tmp = Cc.Split(',');
            if (CcList == null)
                CcList = new List<UsuarioModel>();
            else
                CcList.Clear();
            foreach (string s in tmp)
                if (s != "")
                    CcList.Add(new UsuarioModel { Email = s });
        }

        public void SetBcc(string sbcc = "")
        {
            if (sbcc != "")
                Bcc = sbcc;
            Bcc = Bcc ?? "";
            if (Bcc == "")
                return;
            string[] tmp = Bcc.Split(',');
            if (BccList == null)
                BccList = new List<UsuarioModel>();
            else
                BccList.Clear();
            foreach (string s in tmp)
                if (s != "")
                    BccList.Add(new UsuarioModel { Email = s });
        }

    }
}