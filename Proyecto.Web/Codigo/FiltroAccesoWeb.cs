using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Mgk.Base.ControlAcceso;
using Mgk.Commonsx;
using Mgk.Base.Parametro;
using Mgk.Base.WebCore.Codigo;
using Mgk.DataBasex;

namespace Mgk.Base.WebCore.ActionFilters
{
    public class FiltroAccesoWeb: ActionFilterAttribute
    {
        ControlAccesoCtrl ControlAccesoC = new ControlAccesoCtrl();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            MgkLog.Steep("FiltroAccesoWeb.OnActionExecuting", "<<<<<<<< INICIO --");

            String Authorization = context.HttpContext.Request.Headers["Authorization"];
            String DidHeader = context.HttpContext.Request.Headers["Did"];

            Authorization = Authorization ?? "";
            String ActionName = "";
            String ControllerName = "";
            String Ruta = "";

            try
            {
                ActionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
                ControllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;                
            }
            catch(Exception)
            {
                ActionName = "x";
                ControllerName = "x";
            }

            Ruta = ControllerName + "/" + ActionName;

            MgkLog.Steep("FiltroAccesoWeb.OnActionExecuting", "<<<<<<<< INICIO --["+ Authorization + "] RUTA::"+ Ruta );


            String user = "";
            String password = "";
            int Acceso_id = 0;
            string Did = "";
            if (Authorization != null && DidHeader != null)
            {
                try
                {
                    string[] auth = Authorization.Split(' ');
                    user = MgkFunctions.Base64DeCode(auth[1]);
                    auth = user.Split(':');
                    user = auth[0];
                    password = auth[1];

                    //user = MgkSecret1._decode64(user + password, MgkSecret1.MD5(MgkSecret1.MD5(user + password)));
                    //auth = user.Split(':');

                    //user = auth[0];
                    //password = auth[1];
                    //did = auth[2];
                    //Acceso_id = MgkFunctions.StrToInt(auth[3]);
                    //Did = auth[0];
                    //Acceso_id = MgkFunctions.StrToInt(auth[1]);

                    //if (DidHeader != Did)
                    //{
                    //    Acceso_id = 0;
                    //    user = "";
                    //    password = "";
                    //}
                }
                catch (Exception ex)
                {
                    Acceso_id = 0;
                    user = "";
                    password = "";
                }                
            }

            #region Validacion de acceso por Manejo de sesion
            // << Validacion de acceso por Manejo de sesion
            if (context.HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) != null)
            {
                try
                {
                    AccesoModel AccesoM = context.HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);

                    Boolean Permitido = false;
                    if (AccesoM != null && AccesoM._UsuarioMo != null && AccesoM._UsuarioMo._GrupouList != null)
                    {
                        Permitido = AccesoM._UsuarioMo._ModuloList.Exists(M => M.Ruta.ToLower() == Ruta.ToLower());
                    }

                    if (Permitido == false)
                    {
                        //context.HttpContext.Session.SetString(AppConstantes.SESION_NOMBRE, null);
                        context.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                        ControlAccesoC.Message.Clear();
                        ControlAccesoC.Message.Number = -200;
                        ControlAccesoC.Message.Message = "Intento de acceso no autorizado";
                    }
                }
                catch (Exception exx)
                {
                    //context.HttpContext.Session.SetString(AppConstantes.SESION_NOMBRE, null);
                    context.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                    ControlAccesoC.Message.Clear();
                    ControlAccesoC.Message.Number = -200;
                    ControlAccesoC.Message.Message = "Error al obtener sesión de usuario";
                }
                return;
            }
            // >> Validacion de acceso por Manejo de sesion
            #endregion Validacion de acceso por Manejo de sesion


            if (Acceso_id == 0 && user == "" && password == "")
            {
                context.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                context.HttpContext.Response.StatusCode = 600;
            }
            else
            {
                // identificador pseudoaleatorio de pruebas
                //HttpContext.Current.Session[AppConstantes.SESION_NOMBRE] = DateTime.Now.Millisecond;

                MgkMessage Message = ControlAccesoC.Validar(user, password, context.HttpContext.Request.Host.Host, Acceso_id, Did);

                // Manejo de sesion
                if (ControlAccesoC.Message.Number < 0)
                {
                    context.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                    context.HttpContext.Response.StatusCode = 600;
                }
                else
                {
                    context.HttpContext.Session.SetObjectAsJson(AppConstantes.SESION_NOMBRE, Message.OData);
                }
            }

            // do something before the action executes
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            MgkLog.Steep("FiltroAccesoWeb.OnActionExecuted", "-- FIN >>>>>>>");

            GC.Collect();
            //MgkDataBase.ResumenPool();
            // do something after the action executes
        }
    }
}
