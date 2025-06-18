using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.ControlAcceso
{
    public class AccesoDetalleModel
    {
        public static string __table_name { get; } = "_l_acceso_detalle";
        public static string[] __table_keys { get; } = { "Acceso_detalle_id" };
        public static bool __auto_increment { get; } = true;

        public int Acceso_detalle_id { get; set; }
        public int Acceso_id { get; set; }
        public DateTime? Fecha { get; set; }
        public String Ruta { get; set; }
        public String Method { get; set; }
        public String Datos { get; set; }

    }
}