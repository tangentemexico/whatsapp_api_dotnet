using Mgk.Commonsx;
using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Commonsx;

namespace Mgk.Base.Menu
{
    public class MenuCtrl
    {
        protected MenuDao MenuD { get; set; }

        public MgkMessage Message { get; set; }

        public MenuCtrl()
        {
            Message = new MgkMessage();
            MenuD = new MenuDao();
        }

        /// <summary>
        /// Insertar Menu
        /// </summary>
        /// <param name="MenuM"></param>
        /// <returns></returns>
        public MgkMessage Insert(MenuModel MenuM, AccesoModel AccesoM)
        {
            Message = MenuD.Insert(MenuM, AccesoM.Acceso_id);
            return Message;
        }

        /// <summary>
        /// Actualizar Menu
        /// </summary>
        /// <param name="MenuM"></param>
        /// <returns></returns>
        public MgkMessage Update(MenuModel MenuM, AccesoModel AccesoM)
        {
            Message = MenuD.Update(MenuM, AccesoM.Acceso_id);
            return Message;
        }


        public MgkMessage Save(MenuModel MenuM, AccesoModel AccesoM)
        {
            var mx = Get(MenuM, AccesoM);
            if (mx.OData != null)
                Message = Update(MenuM, AccesoM);
            else
                Message = Insert(MenuM, AccesoM);

            return Message;
        }

        /// <summary>
        /// Eliminar Menu
        /// </summary>
        /// <param name="MenuM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Delete(MenuModel MenuM, AccesoModel AccesoM)
        {
            Message = MenuD.Delete(MenuM, AccesoM.Acceso_id);
            return Message;
        }


        public MgkMessage Get(MenuModel MenuM, AccesoModel AccesoM)
        {
            MenuModel MenuItem = MenuD.GetItem(MenuM, AccesoM.Acceso_id);
            Message = MenuD.Message;
            Message.OData = MenuItem;
            return Message;
        }

        /// <summary>
        /// Leer Lista de Menus
        /// </summary>
        /// <param name="MenuM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public List<MenuModel> GetAll(MenuModel MenuM, AccesoModel AccesoM)
        {
            List<MenuModel> MenuItems = MenuD.GetItems(MenuM, AccesoM.Acceso_id);
            Message = MenuD.Message;

            return MenuItems;
        }

        public List<MenuModel> GetAll(int Acceso_id)
        {
            List<MenuModel> MenuItems = MenuD.GetItems(new MenuModel(), Acceso_id);
            Message = MenuD.Message;

            return MenuItems;
        }
    }
}