using Mgk.Base.ControlAcceso;
using Mgk.Commonsx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.MgkSmtp
{
    public class MensajeriaCtrl
    {
        const String MODULO_NOMBRE = "Mensajeria";
        protected MensajeriaDao MensajeriaD { get; set; }

        public MgkMessage Message { get; set; }

        public MensajeriaCtrl()
        {
            Message = new MgkMessage();
            MensajeriaD = new MensajeriaDao();
        }

        public MgkMessage Send(MgkSmtpModel emailSendModel, AccesoModel AccesoMod)
        {
            MgkSmtpCtrl MgkSmtp = new MgkSmtpCtrl();
            MensajeriaModel MensajeriaM = new MensajeriaModel
            {
                Destinatario = emailSendModel.To,
                Asunto = emailSendModel.Subject,
                _Mensaje = emailSendModel.Body,
                Es_enviado = false,
                Es_adjuntos = (emailSendModel.AttachPathList != null && emailSendModel.AttachPathList.Count > 0)
            };
            this.Guardar(MensajeriaM, AccesoMod);
            MgkMessage Message = MgkSmtp.Send(emailSendModel);

            MensajeriaM.Es_enviado = Message.Number >= 0;
            MensajeriaM.Error_txt = Message.Message+ ";"+Message.Exception??"";
            this.Guardar(MensajeriaM, AccesoMod);
            return Message;
        }

        public MgkMessage Guardar(MensajeriaModel MensajeriaM, AccesoModel AccesoM)
        {
            if (MensajeriaM.Mensajeria_id == 0)
                Message = this.Insert(MensajeriaM, AccesoM);
            else
                Message = this.Update(MensajeriaM, AccesoM);
            return Message;
        }

        /// <summary>
        /// Insertar Mensajeria
        /// </summary>
        /// <param name="MensajeriaM"></param>
        /// <returns></returns>
        public MgkMessage Insert(MensajeriaModel MensajeriaM, AccesoModel AccesoM)
        {
            Message = MensajeriaD.Insert(MensajeriaM, AccesoM);
            return Message;
        }

        /// <summary>
        /// Actualizar Mensajeria
        /// </summary>
        /// <param name="MensajeriaM"></param>
        /// <returns></returns>
        public MgkMessage Update(MensajeriaModel MensajeriaM, AccesoModel AccesoM)
        {
            Message = MensajeriaD.Update(MensajeriaM, AccesoM);
            return Message;
        }

        /// <summary>
        /// Eliminar Mensajeria
        /// </summary>
        /// <param name="MensajeriaM"></param>
        /// <param name="MensajeriaM"></param>
        /// <returns></returns>
        public MgkMessage Delete(MensajeriaModel MensajeriaM, AccesoModel AccesoM)
        {
            Message = MensajeriaD.Delete(MensajeriaM, AccesoM);
            return Message;
        }


        public MgkMessage Get(MensajeriaModel MensajeriaM, AccesoModel AccesoM)
        {
            MensajeriaModel MensajeriaItem = MensajeriaD.GetItem(MensajeriaM, AccesoM);
            Message = MensajeriaD.Message;
            Message.OData = MensajeriaItem;
            return Message;
        }

    }
}