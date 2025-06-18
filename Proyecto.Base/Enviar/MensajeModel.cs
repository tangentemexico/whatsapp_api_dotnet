using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsappControl.Enviar
{
    public class MensajeModel
    {
        public static string __table_name { get; } = "mensajes";
        public static string[] __table_keys { get; } = { "Mensaje_id" };
        public static bool __auto_increment { get; } = true;
        public int Mensaje_id { get; set; }
        public String usuario_id { get; set; }
        public String Origen { get; set; }
        public String Remitente { get; set; }
        public DateTime? Fecha_inserta { get; set; }
        public String Destinatarios { get; set; }
        public String Mensaje { get; set; }
        public String Archivo_url { get; set; }
        public String Archivo_local { get; set; }
        public String Respuesta { get; set; }
        public String Respuesta_subid { get; set; }
        public String Respuesta_code { get; set; }
        public String Respuesta_message { get; set; }

        public DateTime? Fecha_envio { get; set; }
        public String PaisDestino { get; set; }
        public Boolean? Es_enviado { get; set; }

        public int Intentos { get; set; }   

        public string _url_service { get; set; }
    }
}
