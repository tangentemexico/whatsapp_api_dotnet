using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Base.Menu;
using Mgk.Base.Parametro;
using Mgk.Base.WebCore.ActionFilters;
using Mgk.Base.WebCore.Codigo;
using Mgk.Commonsx;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mgk.Base.WebCore.Controllers.MgkRest
{
    [Route("api/[controller]")]
    [ApiController]
    [FiltroAccesoApi]
    public class AMenuController : ControllerBase
    {
        [Route("~/api/AMenu/Menus")]
        [HttpGet(Name = "GetMenu")]
        public async Task<MgkMessage> Get()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            MenuCtrl MenuC = new MenuCtrl();
            MenuModel MenuM = new MenuModel();
            List<MenuModel> Items = MenuC.GetAll(MenuM, AccesoM);
            MenuC.Message.OData = Items;
            return MenuC.Message;
        }

        // GET: api/AMenu/5
        [HttpGet("{id}", Name = "GetMenuId")]
        public async Task<MgkMessage> Get(int id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            MenuCtrl MenuC = new MenuCtrl();
            MenuC.Message.OData = MenuC.Get(new MenuModel { Menu_id = id }, AccesoM);
            return MenuC.Message;
        }

        [Route("~/api/AMenu/Guardar")]
        [HttpPost]
        public async Task<MgkMessage> Guardar([FromBody] MenuModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            MenuCtrl MenuC = new MenuCtrl();
            return MenuC.Save(value, AccesoM);
        }


        // Delete: api/AMenu/5
        [HttpDelete("{id}", Name = "DeleteMenu")]
        public async Task<MgkMessage> Delete(int id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            MenuCtrl MenuC = new MenuCtrl();
            return MenuC.Delete(new MenuModel { Menu_id = id }, AccesoM);
        }
    }
}