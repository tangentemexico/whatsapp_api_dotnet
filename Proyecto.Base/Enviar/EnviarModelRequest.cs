using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Commonsx;

namespace WhatsappControl.Enviar
{
    public class EnviarModelRequest
    {
        public string Usuario_id { get; set; }
        public string Password { get; set; }
        public string PaisDestino { get; set; }
        public string Remitente { get; set; }
        public string Destinatarios { get; set; }
        public string Mensaje { get; set; }
        public string Origen { get; set; }

        public string Archivo_url { get; set; }
        public MgkFilesModel Archivo_local { get; set; }

    }
}
