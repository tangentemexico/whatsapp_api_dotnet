using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Base.MgkSmtp;
using Mgk.Base.Parametro;
using Mgk.Base.Token;
using Mgk.Base.Usuario;
using Mgk.Base.WebCore.Codigo;
using Mgk.Commonsx;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Proyecto.Base.MgkControlAcceso;

namespace Mgk.Base.WebCore.Controllers.MgkRest
{
    [Route("api/[controller]")]
    [ApiController]
    //[FiltroAccesoApi]
    public class AAccesoController : ControllerBase
    {
        string GetUserIp(HttpContext HttpContext) {
            //return HttpContext.Connection.LocalIpAddress.ToString();
            return HttpContext.Request.Host.Host;
        }
        // GET: api/AAcceso
        [HttpGet]
        public IEnumerable<string> Get()
        {
            AccesoModel AccesoMo = HttpContext.Request.HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            return new string[] { "value1", "value2" };
        }

        [Route("~/api/AAcceso/GetConectado")]
        [HttpPost]
        public async Task<AccesoModel> GetConectado()
        {
            AccesoModel AccesoMo = HttpContext.Request.HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            return AccesoMo;
        }

        [Route("~/api/AAcceso/ValidarActualizarPassword")]
        [HttpPost]
        public async Task<MgkMessage> ValidarActualizarPassword([FromBody] TokenModel TokenMo)
        {
            return (new ControlAccesoCtrl()).ValidarActualizarPassword(TokenMo.Token);
        }

        [Route("~/api/AAcceso/ActualizarPassword")]
        [HttpPut]
        public async Task<MgkMessage> ActualizarPassword([FromBody] UsuarioModel value)
        {
            return (new ControlAccesoCtrl()).ActualizarPassword(value);
        }

        [Route("~/api/AAcceso/Invitado")]
        [HttpPost]
        public async Task<MgkMessage> AccesoInvitado()
        {
            UsuarioModel value = new UsuarioModel();
            value.Usuario_id = ParametroCtrl.GetValue("invitado_usr");
            value.Password = ParametroCtrl.GetValue("invitado_pwd");

            String Origen = GetUserIp(HttpContext);
            //String Origen = HttpContext.Request.Host.Host;
            ControlAccesoCtrl ControlAccesoC = new ControlAccesoCtrl();
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
            {
                UsuarioModel UsuarioMod = new UsuarioModel();
                UsuarioMod.Usuario_id = value.Usuario_id;
                UsuarioMod.Password = value.Password;
                try
                {
                    ControlAccesoC.Login(UsuarioMod, Origen);
                }
                catch (Exception ex)
                {

                }

                HttpContext.Session.SetString(AppConstantes.SESION_MSG_INICIO, ParametroCtrl.GetValue("msg_inicio", ""));
                if (ControlAccesoC.Message.Number > 0)
                {
                    HttpContext.Request.HttpContext.Session.SetObjectAsJson(AppConstantes.SESION_NOMBRE, ControlAccesoC.Message.OData);
                }
            }
            else
            {
                try
                {
                    AccesoModel AccesoMo = HttpContext.Request.HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);
                    ControlAccesoC.Message.Number = AccesoMo.Acceso_id;
                    ControlAccesoC.Message.OData = AccesoMo;
                }
                catch (Exception exx)
                {
                    HttpContext.Request.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                    ControlAccesoC.Message.Clear();
                    ControlAccesoC.Message.Number = -200;
                    ControlAccesoC.Message.Message = "Error al obtener sesión de usuario";
                }

            }

            return ControlAccesoC.Message;
        }

        // POST: api/AAcceso
        [HttpPost]
        public async Task<MgkMessage> Post([FromBody] LoginRequestModel value)
        {
            String Origen = GetUserIp(HttpContext);
            //String Origen = HttpContext.Request.Host.Host;
            ControlAccesoCtrl ControlAccesoC = new ControlAccesoCtrl();
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
            {
                UsuarioModel UsuarioMod = new UsuarioModel();
                UsuarioMod.Usuario_id = value.Usuario_id;
                UsuarioMod.Password = value.Password;
                try
                {
                    ControlAccesoC.Login(UsuarioMod, Origen, value.Did,false, value.OsName);
                }
                catch (Exception ex)
                {

                }

                HttpContext.Session.SetString(AppConstantes.SESION_MSG_INICIO, ParametroCtrl.GetValue("msg_inicio", ""));
                if (ControlAccesoC.Message.Number > 0)
                {
                    HttpContext.Request.HttpContext.Session.SetObjectAsJson(AppConstantes.SESION_NOMBRE, ControlAccesoC.Message.OData);
                }
            }
            else
            {
                try
                {
                    AccesoModel AccesoMo = HttpContext.Request.HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);
                    ControlAccesoC.Message.Number = AccesoMo.Acceso_id;
                    ControlAccesoC.Message.OData = AccesoMo;
                    ControlAccesoC.Message.Code = AccesoMo._Acceso_id;
                }
                catch (Exception exx)
                {
                    HttpContext.Request.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                    ControlAccesoC.Message.Clear();
                    ControlAccesoC.Message.Number = -200;
                    ControlAccesoC.Message.Message = "Error al obtener sesión de usuario";
                }

            }

            return ControlAccesoC.Message;
        }

        [Route("~/api/AAcceso/RecuperarPassword")]
        [HttpPost]
        public async Task<MgkMessage> RecuperarPassword([FromBody] UsuarioModel value)
        {
            String Origen = GetUserIp(HttpContext);
            //String Origen = HttpContext.Request.Host.Host;
            MgkMessage Message = ControlAccesoCtrl.SolicitaRecuperarPassword(value);
            return Message;
        }

        [Route("~/api/AAcceso/AccesoWeb")]
        [HttpPost]
        public async Task<MgkMessage> AccesoWeb([FromBody] UsuarioModel value)
        {
            String Origen = GetUserIp(HttpContext);
            //String Origen = HttpContext.Request.Host.Host;
            MgkMessage Message = ControlAccesoCtrl.SolicitaRecuperarPassword(value);
            return Message;
        }

        [Route("~/api/AAcceso/EnviarCredenciales")]
        [HttpPost]
        public async Task<MgkMessage> EnviarCredenciales([FromBody] UsuarioModel value)
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return AppConstantes.MESSAGE_NO_PERMITIDO;

            AccesoModel AccesoM = HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

            UsuarioCtrl oCtrl = new UsuarioCtrl();
            MgkMessage Message = oCtrl.EnviarCredenciales(value);
            return Message;
        }

        [Route("~/api/AAcceso/Salir")]
        [HttpPost]
        public async Task<MgkMessage> Salir()
        {
            MgkMessage Message = new MgkMessage();
            Message.Message = "No existia sesion";
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) != null)
            {
                //HttpContext.Request.HttpContext.Session.Set(AppConstantes.SESION_NOMBRE, null);
                HttpContext.Request.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                Message.Message = "Sesion terminada";
            }
            return Message;
        }

        [Route("~/api/AAcceso/SendMail")]
        [HttpPost]
        public async Task<MgkMessage> SendMail([FromBody] MgkSmtpModel eMod)
        {
            AccesoModel AccesoMo = HttpContext.Request.HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);
            MgkMessage mess = new MgkMessage();
            if (string.IsNullOrEmpty(eMod.Subject))
            {
                eMod.Parameters = new EmailParameters();
                mess.OData = new { eMod = eMod };
                return mess;
            }
            eMod.Parameters.bSmtpStatus = true;
            MgkSmtpCtrl MgkSmtp = new MgkSmtpCtrl();
            MgkSmtp.SetParameters(eMod.Parameters);  
            return MgkSmtp.Send(eMod, "smtp");
        }

        //// DELETE: api/AAcceso/5
        //public void Delete(int id)
        //{
        //    if (HttpContext.Current.Session[Constantes.SESION_NOMBRE] != null)
        //    {
        //        //HttpContext.Current.Session[Constantes.SESION_NOMBRE] = ControlAcceso.Message.Number;
        //    }
        //}

    }
}