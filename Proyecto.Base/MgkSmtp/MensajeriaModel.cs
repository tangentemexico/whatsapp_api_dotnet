using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.MgkSmtp
{
    public class MensajeriaModel
    {
        public static string __table_name { get; } = "t_mensajeria";
        public static string[] __table_keys { get; } = { "Mensajeria_id" };
        public static bool __auto_increment { get; } = true;

        public int Mensajeria_id { get; set; }
        public DateTime Fecha_inserta { get; set; }
        public String Usuario_id_inserta { get; set; }
        public String Remitente { get; set; }
        public String Destinatario { get; set; }
        public String Asunto { get; set; }
        public String _Mensaje
        {
            get
            {
                return System.Text.Encoding.UTF8.GetString(Mensaje);
            }
            set
            {
                this.Mensaje = Encoding.UTF8.GetBytes(value);
            }
        }
        public byte[] Mensaje { get; set; }
        public String Modo_envio { get; set; }
        public Boolean? Es_enviado { get; set; }
        public Boolean? Es_recibido { get; set; }
        public Boolean? Es_leido { get; set; }
        public Boolean? Es_adjuntos { get; set; }
        public String Error_txt { get; set; }
    }
}
