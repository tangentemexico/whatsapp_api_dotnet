using System;
using System.Collections.Generic;
using System.Text;

namespace Mgk.Commonsx
{
    public class MgkMessages : MgkMessage
    {
        public List<MgkMessage> MessagesList;
        public int TotalDanger = 0;
        public int TotalWarning = 0;
        public int TotalSuccess = 0;
        public int TotalInfo = 0;
        public int TotalNoSuccess = 0;

        public void Add(MgkMessage M)
        {
            this.SetMessage(M);
            M.SetMessage(M);
            if (MessagesList == null)
                MessagesList = new List<MgkMessage>();
            switch (M.Type)
            {
                case MgkMessage.TYPE_DANGER:
                    TotalDanger++;
                    break;
                case MgkMessage.TYPE_WARNING:
                    TotalWarning++;
                    break;
                case MgkMessage.TYPE_SUCCESS:
                    TotalSuccess++;
                    break;
                case MgkMessage.TYPE_INFO:
                    TotalInfo++;
                    break;
            }
            MessagesList.Add(M);
            TotalNoSuccess = MessagesList.Count - TotalSuccess;
        }

        /// <summary>
        /// Agregar todos los mensajes recibidos en la lista local de mensajes
        /// </summary>
        /// <param name="Messages"></param>
        public void AddList(MgkMessages Messages)
        {
            if (Messages != null && Messages.MessagesList != null)
                foreach (MgkMessage Message in Messages.MessagesList)
                    this.Add(Message);
        }

        public void AddException(Exception Ex, string Source, string Method, string Message = "")
        {
            MgkMessage M = new MgkMessage
            {
                Code = "Exception",
                Type = MgkMessage.TYPE_DANGER,
                Number = MgkResponseCode.SYS_EXCEPTION_UNKNOW,
                Source = Ex.Source,
                Message = Message == "" ? Ex.ToString() : Message,
                Message2 = Source + "-" + Method,
                Message3 = Message != "" ? Ex.ToString() : Message,
            };
            this.SetMessage(M);
            M.SetMessage(M);
            Add(M);
        }

        public void AddError(string Source, string Method, string Message)
        {
            MgkMessage M = new MgkMessage
            {
                Code = "Error",
                Type = MgkMessage.TYPE_DANGER,
                Number = -1,
                Source = Source + "-" + Method,
                Message = Message,
            };
            this.SetMessage(M);
            M.SetMessage(M);
            Add(M);
        }

        /// <summary>
        /// Devuelve la lista de MessagesList
        /// </summary>
        /// <returns></returns>
        public List<MgkMessage> GetMessages()
        {
            return MessagesList;
        }

        /// <summary>
        /// Limpiar la lista de MessagesList
        /// </summary>
        public void ClearMessages()
        {
            this.Clear();
            MgkStaticMessage.Clear();
            if (MessagesList == null)
                MessagesList = new List<MgkMessage>();
            else
                MessagesList.Clear();

        TotalDanger = 0;
        TotalWarning = 0;
        TotalSuccess = 0;
        TotalInfo = 0;
        TotalNoSuccess = 0;
    }

        /// <summary>
        /// Devuelve el último Message agregado, si no existe devuelve NULL
        /// </summary>
        /// <returns></returns>
        public MgkMessage GetLastMessage()
        {
            if (MessagesList != null && MessagesList.Count > 0)
                return MessagesList[MessagesList.Count - 1];
            else
                return new MgkMessage();
        }

    }
}
