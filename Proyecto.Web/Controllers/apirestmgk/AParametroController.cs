using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Base.Grupou;
using Mgk.Base.Menu;
using Mgk.Base.Modulo;
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
    public class AParametroController : ControllerBase
    {
        // GET: api/AParametro/Listado
        [Route("~/api/AParametro/Listado")]
        [HttpGet]
        public async Task<MgkMessage> Get()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ParametroCtrl ParametroC = new ParametroCtrl();
            var Items = ParametroC.GetAll(AccesoM);
            ParametroC.Message.OData = Items;
            return ParametroC.Message;
        }

        // GET: api/AParametro/5
        [Route("~/api/AParametro/Get/")]
        [HttpGet("{id}", Name = "GetParametro")]
        public async Task<MgkMessage> Get(String id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ParametroCtrl ParametroC = new ParametroCtrl();

            if (id.IndexOf(',')>=0)
                ParametroC.Message.OData = ParametroC.GetAll(new ParametroModel { Parametro_id = id }, AccesoM);
            else
                ParametroC.Message.OData = ParametroC.Get(new ParametroModel { Parametro_id = id }, AccesoM);
            return ParametroC.Message;
        }


        // POST: api/AParametro
        [HttpPost]
        public async Task<MgkMessage> Post([FromBody]ParametroModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ParametroCtrl ParametroC = new ParametroCtrl();
            return ParametroC.Save(value, AccesoM);
        }

        // PUT: api/AParametro/5
        [HttpPut]
        public async Task<MgkMessage> Put(String id, [FromBody]ParametroModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ParametroCtrl ParametroC = new ParametroCtrl();
            value.Parametro_id = id;
            return ParametroC.Update(value, AccesoM);
        }

        // Delete: api/AParametro/5
        [HttpDelete("{id}", Name = "DeleteParametro")]
        public async Task<MgkMessage> Delete(String id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            ParametroCtrl ParametroC = new ParametroCtrl();
            return ParametroC.Delete(new ParametroModel { Parametro_id = id }, AccesoM);
        }

        // GET: api/AParametro/AppSettings
        [Route("~/api/AParametro/AppSettings")]
        [HttpGet]
        public async Task<MgkMessage> GetAppSettings()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);
            MgkMessage Message = new MgkMessage();
            Message.OData = MgkFunctions.GetAllSettings();
            return Message;
        }

    }
}
