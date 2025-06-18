using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsappControl.Enviar
{
    public class UsuarioServicioModel
    {
        public static string __table_name { get; } = "usuario_servicio";
        public static string[] __table_keys { get; } = { "usuario_id" };
        public static bool __auto_increment { get; } = false;
        public string usuario_id { get; set; }
        public string url { get; set; }
    }
}
