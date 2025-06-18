using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Base.Grupou;
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
    public class AGrupouController : ControllerBase
    {
        [Route("~/api/AGrupou/Grupous")]
        [HttpGet(Name = "GetGrupou")]
        public async Task<MgkMessage> Get()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            GrupouCtrl GrupouC = new GrupouCtrl();
            GrupouModel GrupouM = new GrupouModel();
            List<GrupouModel> Items = GrupouC.GetAll(GrupouM, AccesoM);
            GrupouC.Message.OData = Items;
            return GrupouC.Message;
        }

        // GET: api/AGrupou/5
        [HttpGet("{id}", Name = "GetGrupouId")]
        public async Task<MgkMessage> Get(string id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            GrupouCtrl GrupouC = new GrupouCtrl();
            GrupouC.Message.OData = GrupouC.Get(new GrupouModel { Grupou_code = id }, AccesoM);
            return GrupouC.Message;
        }

        [Route("~/api/AGrupou/Guardar")]
        [HttpPost]
        public async Task<MgkMessage> Guardar([FromBody] GrupouModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            GrupouCtrl GrupouC = new GrupouCtrl();
            return GrupouC.Save(value, AccesoM);
        }


        // Delete: api/AGrupou/5
        [HttpDelete("{id}", Name = "DeleteGrupou")]
        public async Task<MgkMessage> Delete(string id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            GrupouCtrl GrupouC = new GrupouCtrl();
            return GrupouC.Delete(new GrupouModel { Grupou_code = id }, AccesoM);
        }
    }
}