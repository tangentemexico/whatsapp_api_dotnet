using Mgk.Commonsx;
using Mgk.DataBasex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsappControl.Enviar
{
    public class EnviarDao:MgkDataBaseObjT
    {
        public MgkMessage Guardar(MensajeModel MensajeMod)
        {
            MensajeMod.Fecha_inserta = DateTime.Now;
            this.Insert(MensajeMod);
            return this.Messages.GetLastMessage();
        }
        public List<MensajeModel> NoEnviados() 
        {
            List<MensajeModel> items = this.ReadObjListByQuery<MensajeModel>(@$"SELECT m.* 
,u.url as _url_service
FROM mensajes as m join usuario_servicio as u on u.usuario_id=m.usuario_id
where IFNULL(m.Es_enviado,0) =0 and m.Intentos < 10");
            return items;
        }

        public MgkMessage ActualizarFechaEnvio(MensajeModel MensajeMod)
        {
            MensajeMod.Fecha_envio = DateTime.Now;
            MensajeMod.Intentos++;
            this.Update(MensajeMod, "Fecha_envio,Respuesta_message,Intentos,Es_enviado");
            return this.Messages.GetLastMessage();
        }
        
    }
}
