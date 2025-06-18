using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.HistoriaTabla
{
    public class HistoriaTablaModel
    {
        public const String OPERACION_INSERTAR = "I";
        public const String OPERACION_LEER = "R";
        public const String OPERACION_ACTUALIZAR = "U";
        public const String OPERACION_BORRAR = "D";

        public static string __table_name { get; } = "_l_historia_tabla";
        public static string[] __table_keys { get; } = { "Historia_tabla_id" };
        public static bool __auto_increment { get; } = true;

        public int Historia_tabla_id { get; set; }
        public String Nombre { get; set; }
        public String Llave { get; set; }
        public String Operacion { get; set; }
        public int Acceso_id { get; set; }
        public String Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }
}