using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.ControlAcceso
{
    public class MgkSessionAccesoModel
    {
        public static string __table_name { get; } = "_session";
        public static string[] __table_keys { get; } = { "Acceso_id" };
        public static bool __auto_increment { get; } = false;

        public int Acceso_id { get; set; }
        public String Session_json { get; set; }

        public String Usuario_id { get; set; }
        public String Front { get; set; }
    }
}
