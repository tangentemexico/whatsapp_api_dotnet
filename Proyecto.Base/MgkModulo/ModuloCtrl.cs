using Mgk.Commonsx;
using Mgk.Base.ControlAcceso;
using Mgk.Base.HistoriaTabla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Modulo
{
    public class ModuloCtrl
    {
        const String MODULO_NOMBRE = "modulo";
        private ModuloDao ModuloD { get; set; }

        public MgkMessage Message { get; set; }

        public ModuloCtrl()
        {
            Message = new MgkMessage();
            ModuloD = new ModuloDao();
        }

        /// <summary>
        /// Antes de guardar veririfica si existe para hacer update o se trata de un registro nuevo
        /// </summary>
        /// <param name="ModuloMod"></param>
        /// <param name="AccesoM"></param>
        /// <returns></returns>
        public MgkMessage Save(ModuloModel ModuloMod, AccesoModel AccesoM)
        {
            if (ModuloMod.Ruta == null)
                ModuloMod.Ruta = "";
            if (ModuloMod.RutaApi == null)
                ModuloMod.RutaApi = "";

            ModuloModel ModuloModNew = ModuloD.ReadObject<ModuloModel>(ModuloMod, null, true);
            if (ModuloModNew != null && (ModuloModNew.Modulo_cod??"") != "")
                return this.Update(ModuloMod, AccesoM);
            else
                return this.Insert(ModuloMod, AccesoM);
        }

        /// <summary>
        /// Insertar Modulo
        /// </summary>
        /// <param name="ModuloM"></param>
        /// <returns></returns>
        public MgkMessage Insert(ModuloModel ModuloM, AccesoModel AccesoM)
        {
            Message = ModuloD.Insert(ModuloM, AccesoM);

            if (Message.Number >= 0)
                HistoriaTablaCtrl.Insert(new HistoriaTablaModel
                {
                    Nombre = ModuloModel.__table_name,
                    Llave = ModuloM.Modulo_cod,
                    Operacion = HistoriaTablaModel.OPERACION_INSERTAR
                }, AccesoM);

            return Message;
        }

        /// <summary>
        /// Actualizar Modulo
        /// </summary>
        /// <param name="ModuloM"></param>
        /// <returns></returns>
        public MgkMessage Update(ModuloModel ModuloM, AccesoModel AccesoM)
        {
            Message = ModuloD.Update(ModuloM, AccesoM);
            if (Message.Number >= 0)
                HistoriaTablaCtrl.Insert(new HistoriaTablaModel
                {
                    Nombre = ModuloModel.__table_name,
                    Llave = ModuloM.Modulo_cod,
                    Operacion = HistoriaTablaModel.OPERACION_ACTUALIZAR
                }, AccesoM);
            return Message;
        }

        /// <summary>
        /// Eliminar Modulo
        /// </summary>
        /// <param name="ModuloM"></param>
        /// <param name="ModuloM"></param>
        /// <returns></returns>
        public MgkMessage Delete(ModuloModel ModuloM, AccesoModel AccesoM)
        {
            Message = ModuloD.Delete(ModuloM, AccesoM);

            if (Message.Number >= 0)
                HistoriaTablaCtrl.Insert(new HistoriaTablaModel
                {
                    Nombre = ModuloModel.__table_name,
                    Llave = ModuloM.Modulo_cod,
                    Operacion = HistoriaTablaModel.OPERACION_BORRAR
                }, AccesoM);

            return Message;
        }


        public MgkMessage Get(ModuloModel ModuloM, AccesoModel AccesoM)
        {
            ModuloModel ModuloItem = ModuloD.GetItem(ModuloM, AccesoM);
            Message = ModuloD.Message;
            Message.OData = ModuloItem;
            return Message;
        }


        /// <summary>
        /// Leer Lista de Modulos
        /// </summary>
        /// <param name="ModuloM"></param>
        /// <param name="ModuloM"></param>
        /// <returns></returns>
        public List<ModuloModel> GetAll(ModuloModel ModuloM, AccesoModel AccesoM)
        {
            var ModuloItems = ModuloD.GetItems(ModuloM, AccesoM);
            Message = ModuloD.Message;

            return ModuloItems;
        }

        public List<ModuloModel> GetAll(AccesoModel AccesoM)
        {
            var ModuloItems = ModuloD.GetItems(new ModuloModel(), AccesoM);
            Message = ModuloD.Message;

            return ModuloItems;
        }
    }
}