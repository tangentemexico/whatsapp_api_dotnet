using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Base.Grupou;
using Mgk.Base.Menu;
using Mgk.Base.Modulo;
using Mgk.Base.Parametro;
using Mgk.Base.Usuario;
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
    public class AUsuarioController : ControllerBase
    {
        [Route("~/api/AUsuario/Listado")]
        [HttpPost(Name = "UsuarioListado")]
        public async Task<MgkMessage> Listado([FromBody] UsuarioModel UsuarioM)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;
            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            List<UsuarioModel> Items = UsuarioC.GetAll(UsuarioM, AccesoM.Acceso_id);
            UsuarioC.Message.OData = Items;
            return UsuarioC.Message;
        }

        [Route("~/api/AUsuario/GetUsuarioId")]
        [HttpPost]
        public async Task<MgkMessage> GetUsuarioId([FromBody] UsuarioModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            UsuarioC.Message.OData = UsuarioC.Get(value, AccesoM);
            return UsuarioC.Message;
        }

        [Route("~/api/AUsuario/Guardar")]
        [HttpPost]
        public async Task<MgkMessage> Guardar([FromBody]UsuarioModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            return UsuarioC.Guardar(value, AccesoM);
        }

        [Route("~/api/AUsuario/Perfil")]
        [HttpGet(Name = "Perfil")]
        public async Task<MgkMessage> Perfil()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            AccesoM._UsuarioMo.Password = "";
            return new MgkMessage {
                OData = new { Usuario = AccesoM._UsuarioMo }
            };
        }

  

        // POST: api/AUsuario
        [HttpPost]
        public async Task<MgkMessage> Post([FromBody]UsuarioModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            return UsuarioC.Insert(value, AccesoM);
        }

        [Route("~/api/AUsuario/ActualizarPassword")]
        [HttpPut]
        public async Task<MgkMessage> ActualizarPassword([FromBody]UsuarioModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            String NuevoPassword = value.Password;
            value._Password = value.Usuario_id;
            value._Password = value.Password;

            if (AccesoM._UsuarioMo.Password != value.Password)
            {
                return new MgkMessage
                {
                    Number = -1,
                    Message = "Password actual incorrecto"
                };
            }
            AccesoM._UsuarioMo.Password = NuevoPassword;

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            UsuarioC.ActualizarPassword(AccesoM._UsuarioMo, AccesoM);
            if (UsuarioC.Message.Number >= 0)
            {
                AccesoM._UsuarioMo._Password = NuevoPassword;
                AccesoM._UsuarioMo._Password = AccesoM._UsuarioMo.Password;
                HttpContext.Request.HttpContext.Session.SetObjectAsJson(AppConstantes.SESION_NOMBRE, AccesoM);
            }
            return UsuarioC.Message;
        }

        // PUT: api/AUsuario/5
        [HttpPut]
        public async Task<MgkMessage> Put(string id, [FromBody]UsuarioModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);


            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            value.Usuario_id = id;
            return UsuarioC.Update(value, AccesoM);
        }

        // Delete: api/AUsuario/5
        [HttpDelete("{id}", Name = "DeleteUsuario")]
        public async Task<MgkMessage> Delete(string id)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            return UsuarioC.Delete(new UsuarioModel { Usuario_id = id }, AccesoM);
        }

        //[Route("~/api/AUsuario/DescargaTodo")]
        //[HttpGet(Name = "AUsuarioDescargaTodo")]
        //public FileResult DescargaTodo()
        //{
        //    if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
        //    {
        //        MgkFilesModel Archivox = new MgkFilesModel { FileName = "no_existe", FileExt = "txt" };
        //        Archivox._Bytes = Encoding.ASCII.GetBytes("No encontrado");
        //        Response.Headers.Add("Content-Disposition", "inline; filename=" + Archivox._FileName);
        //        return File(Archivox._Bytes, "text/plain");
        //    }

        //    AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);
        //    UsuarioCtrl oCtrl = new UsuarioCtrl();

        //    MgkFilesModel oArchivo = oCtrl.ExcelTodo();

        //    if (oArchivo != null && oArchivo._Bytes != null)
        //    {
        //        Response.Headers.Add("Content-Disposition", "inline; filename=" + oArchivo.FileName + "." + oArchivo.FileExt);
        //        return File(oArchivo._Bytes, "application/vnd.ms-excel");
        //    }
        //    else
        //    {
        //        MgkFilesModel Archivox = new MgkFilesModel { FileName = "no_existe", FileExt = "txt" };
        //        Archivox._Bytes = Encoding.ASCII.GetBytes("Error");
        //        Response.Headers.Add("Content-Disposition", "inline; filename=" + Archivox._FileName);
        //        return File(Archivox._Bytes, "text/plain");
        //    }
        //}

    }
}