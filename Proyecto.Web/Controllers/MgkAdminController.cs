using Mgk.Base.ControlAcceso;
using Mgk.Base.Parametro;
using Microsoft.AspNetCore.Mvc;

namespace Mgk.Base.WebCore.Controllers
{
    public class MgkAdminController : Controller
    {
        public IActionResult Parametros()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return Redirect(AccesoModel.UrlLogin());
            return View();
        }

        public IActionResult Usuarios()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return Redirect(AccesoModel.UrlLogin());
            return View();
        }

        public IActionResult miusuario()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return Redirect(AccesoModel.UrlLogin());
            return View();
        }

        public IActionResult Menu()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return Redirect(AccesoModel.UrlLogin());
            return View();
        }

        public IActionResult Grupou()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return Redirect(AccesoModel.UrlLogin());
            return View();
        }

        public IActionResult Modulo()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return Redirect(AccesoModel.UrlLogin());
            return View();
        }
    }
}
