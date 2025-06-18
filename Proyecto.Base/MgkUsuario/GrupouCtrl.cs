using Mgk.Commonsx;
using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;

namespace Mgk.Base.Grupou
{
    public class GrupouCtrl
    {
        protected GrupouDao GrupouD { get; set; }

        public MgkMessage Message { get; set; }

        public GrupouCtrl()
        {
            Message = new MgkMessage();
            GrupouD = new GrupouDao();
        }

        /// <summary>
        /// Insertar Grupou
        /// </summary>
        /// <param name="GrupouM"></param>
        /// <returns></returns>
        public MgkMessage Insert(GrupouModel GrupouM, AccesoModel AccesoM)
        {
            Message = GrupouD.Insert(GrupouM, AccesoM.Acceso_id);
            return Message;
        }

        /// <summary>
        /// Actualizar Grupou
        /// </summary>
        /// <param name="GrupouM"></param>
        /// <returns></returns>
        public MgkMessage Update(GrupouModel GrupouM, AccesoModel AccesoM)
        {
            Message = GrupouD.Update(GrupouM, AccesoM.Acceso_id);
            return Message;
        }

        public MgkMessage UpdateMenuLista(GrupouModel GrupouM)
        {
            return GrupouD.UpdateMenuLista(GrupouM);
        }

        public MgkMessage UpdateModuloLista(GrupouModel GrupouM)
        {
            return GrupouD.UpdateModuloLista(GrupouM);
        }

        public MgkMessage Save(GrupouModel GrupouM, AccesoModel AccesoM)
        {
            GrupouModel GrupouItem = GrupouD.ReadNew<GrupouModel>(GrupouM);
            if (GrupouItem!=null)
                Message = Update(GrupouM, AccesoM);
            else
                Message = Insert(GrupouM, AccesoM);

            if (Message.Number >= 0)
            {                
                UpdateMenuLista(GrupouM);
                UpdateModuloLista(GrupouM);
                Message.Message = "Registro guardado exitosamente";
            }

            return Message;
        }

        /// <summary>
        /// Eliminar Grupou
        /// </summary>
        /// <param name="GrupouM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Delete(GrupouModel GrupouM, AccesoModel AccesoM)
        {
            Message = GrupouD.Delete(GrupouM, AccesoM.Acceso_id);
            return Message;
        }


        public MgkMessage Get(GrupouModel GrupouM, AccesoModel AccesoM)
        {
            GrupouModel GrupouItem = GrupouD.GetItem(GrupouM, AccesoM.Acceso_id);
            Message = GrupouD.Message;
            Message.OData = GrupouItem;
            return Message;
        }

        /// <summary>
        /// Leer Lista de Grupous
        /// </summary>
        /// <param name="GrupouM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public List<GrupouModel> GetAll(GrupouModel GrupouM, AccesoModel AccesoM)
        {
            List<GrupouModel> GrupouItems = GrupouD.GetItems(GrupouM, AccesoM.Acceso_id);
            Message = GrupouD.Message;

            return GrupouItems;
        }

        public List<GrupouModel> GetAll(int Acceso_id)
        {
            List<GrupouModel> GrupouItems = GrupouD.GetItems(new GrupouModel(), Acceso_id);
            Message = GrupouD.Message;

            return GrupouItems;
        }
    }
}