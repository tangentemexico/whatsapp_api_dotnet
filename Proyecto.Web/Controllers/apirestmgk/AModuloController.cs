using Mgk.Base.WebCore.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Base.Modulo;
using Mgk.Base.Parametro;

using Mgk.Base.WebCore.Codigo;
using Mgk.Commonsx;
using Microsoft.AspNetCore.Http;


namespace Proyecto.Web.Controllers.apirestmgk
{
    [Route("api/[controller]")]
    [ApiController]
    [FiltroAccesoApi]
    public class AModuloController : Controller
    {

        [Route("~/api/AModulo/Modulos")]
        [HttpGet(Name = "GetModulo")]
        public async Task<MgkMessage> Get()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ModuloCtrl ModuloC = new ModuloCtrl();
           ModuloModel ModuloM = new ModuloModel();
            List<ModuloModel> Items = ModuloC.GetAll(ModuloM, AccesoM);
            ModuloC.Message.OData = Items;
            return ModuloC.Message;
        }

        // GET: api/AModulo/5
        [HttpGet("{id}", Name = "GetModulo_cod")]
        public async Task<MgkMessage> Get(string id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ModuloCtrl ModuloC = new ModuloCtrl();
            ModuloC.Message.OData = ModuloC.Get(new ModuloModel { Modulo_cod = id }, AccesoM);
            return ModuloC.Message;
        }

        [Route("~/api/AModulo/Guardar")]
        [HttpPost]
        public async Task<MgkMessage> Guardar([FromBody] ModuloModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ModuloCtrl ModuloM = new ModuloCtrl();
            return ModuloM.Save(value, AccesoM);
        }


        // Delete: api/AModulo/5
        [HttpDelete("{id}", Name = "DeleteModulo")]
        public async Task<MgkMessage> Delete(string id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ModuloCtrl ModuloM = new ModuloCtrl();
            return ModuloM.Delete(new ModuloModel { Modulo_cod = id }, AccesoM);
        }
    }
}