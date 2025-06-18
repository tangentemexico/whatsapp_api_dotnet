using Mgk.Base.Usuario;
using Mgk.Commonsx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.ControlAcceso
{
    public class RequestUri
    {
        public String AbsolutePath { get; set; }
        public String AbsoluteUri { get; set; }
        public String Autority { get; set; }
        public String DnsSafeHost { get; set; }
        public String Host { get; set; }
        public String HostNameType { get; set; }
        public String IdnHost { get; set; }
        public String LocalPath { get; set; }
        public String OriginalString { get; set; }
        public String PathAndQuery { get; set; }
        public int Port { get; set; }
        public String Scheme { get; set; }
    }

    public class AccesoModel
    {
        public static string __table_name { get; } = "_l_acceso";
        public static string[] __table_keys { get; } = { "Acceso_id" };
        public static bool __auto_increment { get; } = true;

        public int Acceso_id { get; set; }
        public String Usuario_id { get; set; }
        public DateTime? Fecha_inicio { get; set; }
        public DateTime? Fecha_actualiza { get; set; }
        public DateTime? Fecha_fin { get; set; }
        public String Origen { get; set; }
        /// <summary>
        /// Auxiliar Menu
        /// </summary>
        public String Auxx { get; set; }

        //public String _User_name { get; set; }
        public String Did { get; set; }
        public String OsName { get; set; }
        public int _Cliente_id { get; set; }

        public UsuarioModel _UsuarioMo { get; set; }

        public RequestUri _RequestUri { get; set; }

        public String _Empresa_cod { get; set; }
        public String _Session_json { get; set; }

        public String _Acceso_id
        {
            //set{
            //    this.Acceso_id = Mgk.Commonsx.MgkFunctions.StrToInt(value);
            //}
            get
            {
                return (""+ ":" + Acceso_id + ":" + MgkFunctions.Md5i2(Acceso_id.ToString() + "-" + Acceso_id.ToString()));
            }
        }


        public static string UrlLogin()
        {
            return "~/Acceso";
        }

        public static string UrlHome()
        {
            return "~/";
        }
    }
}