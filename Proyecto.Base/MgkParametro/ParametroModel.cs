using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Parametro
{
    public class ParametroModel
    {
        public static string __table_name { get; } = "_Parametro";
        public static string[] __table_keys { get; } = { "Parametro_id" };
        public static bool __auto_increment { get; } = false;

        public String Parametro_id { get; set; }
        public String Valor { get; set; }
        public String Descripcion { get; set; }
        public String Tipo { get; set; }
        public String _clave { get; set; }
    }
}
