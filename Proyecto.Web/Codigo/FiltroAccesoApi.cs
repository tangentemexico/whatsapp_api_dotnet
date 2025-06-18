using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Mgk.Base.ControlAcceso;
using Mgk.Base.Parametro;
using Mgk.Base.WebCore.Codigo;
using Mgk.Commonsx;

namespace Mgk.Base.WebCore.ActionFilters
{
    public class FiltroAccesoApi: ActionFilterAttribute
    {
        ControlAccesoCtrl ControlAccesoC = new ControlAccesoCtrl();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            MgkLog.Steep("FiltroAccesoApi.OnActionExecuting", "<<<<<<<< INICIO --");
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
            catch (Exception)
            {
                ActionName = "x";
                ControllerName = "x";
            }

            Ruta = ControllerName + "/" + ActionName;

            MgkLog.Steep("FiltroAccesoApi.OnActionExecuting", "<<<<<<<< INICIO --[" + Authorization + "] RUTA::" + Ruta);


            String user = "";
            String password = "";
            int Acceso_id = 0;
            string Did = "";
            if (Authorization != null)
            {
                try
                {
                    string[] auth = Authorization.Split(' ');
                    user = auth[1];

                    auth = user.Split(':');
                    user = auth[1];
                    password = auth[2];

                    Acceso_id = MgkFunctions.StrToInt(auth[1]);
                   
                }
                catch (Exception ex)
                {
                    Acceso_id = 0;
                    user = "";
                    password = "";
                }                
            }

            if (context.HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) != null)
            {
                try
                {
                    AccesoModel AccesoM = context.HttpContext.Session.GetObjectFromJson<AccesoModel>(AppConstantes.SESION_NOMBRE);
                }
                catch (Exception exx)
                {
                    context.HttpContext.Session.SetString(AppConstantes.SESION_NOMBRE, null);
                    ControlAccesoC.Message.Clear();
                    ControlAccesoC.Message.Number = -200;
                    ControlAccesoC.Message.Message = "Error al obtener sesión de usuario";
                }
                return;
            }

            if (Acceso_id == 0 && user == "" && password == "")
            {
                context.HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
                context.HttpContext.Response.StatusCode = 600;
            }
            else
            {
                MgkMessage Message = ControlAccesoC.Validar(user, password, context.HttpContext.Request.Host.Host, Acceso_id, Did);
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
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            GC.Collect();
            MgkLog.Steep("FiltroAccesoApi.OnActionExecuted", "-- FIN >>>>>>");
        }
    }
}
