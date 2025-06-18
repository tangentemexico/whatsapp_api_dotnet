using Mgk.Commonsx;
using Mgk.Base.ControlAcceso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Commonsx;

namespace Mgk.Base.HistoriaTabla
{
    public class HistoriaTablaCtrl
    {
        private static HistoriaTablaDao HistoriaTablaD { get; set; }

        public static MgkMessage Message { get; set; }

        public HistoriaTablaCtrl()
        {
            Init();
        }

        private static void Init()
        {
            if (Message == null)
                Message = new MgkMessage();
            if (HistoriaTablaD == null)
                HistoriaTablaD = new HistoriaTablaDao();
        }

        /// <summary>
        /// Antes de guardar veririfica si existe para hacer update o se trata de un registro nuevo
        /// </summary>
        /// <param name="HistoriaTablaMod"></param>
        /// <param name="AccesoM"></param>
        /// <returns></returns>
        public MgkMessage Save(HistoriaTablaModel HistoriaTablaMod, AccesoModel AccesoM)
        {
            if (HistoriaTablaMod.Historia_tabla_id>0)
                return this.Update(HistoriaTablaMod, AccesoM);
            else
                return Insert(HistoriaTablaMod, AccesoM);
        }

        /// <summary>
        /// Insertar HistoriaTabla
        /// </summary>
        /// <param name="HistoriaTablaM"></param>
        /// <returns></returns>
        public static MgkMessage Insert(HistoriaTablaModel HistoriaTablaM, AccesoModel AccesoM)
        {
            Init();

            HistoriaTablaM.Fecha = DateTime.Now;
            HistoriaTablaM.Acceso_id = AccesoM.Acceso_id;
            HistoriaTablaM.Usuario = AccesoM.Usuario_id;
            Message = HistoriaTablaD.Insert(HistoriaTablaM, AccesoM);
            return Message;
        }

        /// <summary>
        /// Actualizar HistoriaTabla
        /// </summary>
        /// <param name="HistoriaTablaM"></param>
        /// <returns></returns>
        public MgkMessage Update(HistoriaTablaModel HistoriaTablaM, AccesoModel AccesoM)
        {
            Message = HistoriaTablaD.Update(HistoriaTablaM, AccesoM);
            return Message;
        }

        /// <summary>
        /// Eliminar HistoriaTabla
        /// </summary>
        /// <param name="HistoriaTablaM"></param>
        /// <param name="HistoriaTablaM"></param>
        /// <returns></returns>
        public MgkMessage Delete(HistoriaTablaModel HistoriaTablaM, AccesoModel AccesoM)
        {
            Message = HistoriaTablaD.Delete(HistoriaTablaM, AccesoM);
            return Message;
        }


        public MgkMessage Get(HistoriaTablaModel HistoriaTablaM, AccesoModel AccesoM)
        {
            HistoriaTablaModel HistoriaTablaItem = HistoriaTablaD.GetItem(HistoriaTablaM, AccesoM);
            Message = HistoriaTablaD.Message;
            Message.OData = HistoriaTablaItem;
            return Message;
        }


        /// <summary>
        /// Leer Lista de HistoriaTablas
        /// </summary>
        /// <param name="HistoriaTablaM"></param>
        /// <param name="HistoriaTablaM"></param>
        /// <returns></returns>
        public List<HistoriaTablaModel> GetAll(HistoriaTablaModel HistoriaTablaM, AccesoModel AccesoM)
        {
            var HistoriaTablaItems = HistoriaTablaD.GetItems(HistoriaTablaM, AccesoM);
            Message = HistoriaTablaD.Message;

            return HistoriaTablaItems;
        }

        public List<HistoriaTablaModel> GetAll(AccesoModel AccesoM)
        {
            var HistoriaTablaItems = HistoriaTablaD.GetItems(new HistoriaTablaModel(), AccesoM);
            Message = HistoriaTablaD.Message;

            return HistoriaTablaItems;
        }
    }
}