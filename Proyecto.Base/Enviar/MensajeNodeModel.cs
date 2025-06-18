using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsappControl.Enviar
{
    public class MensajeNodeModel
    {
        public string PaisDestino { get; set; }
        public string mensaje { get; set; }
        public string numero { get; set; }
        public string Archivo_local { get; set; }
        public string Archivo_url { get; set; }
    }
}
