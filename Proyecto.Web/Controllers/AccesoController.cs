using Mgk.Base.ControlAcceso;
using Mgk.Base.Parametro;
using Microsoft.AspNetCore.Mvc;

namespace Mgk.Base.WebCore.Controllers
{
    public class AccesoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Salir()
        {
            HttpContext.Session.Remove(AppConstantes.SESION_NOMBRE);
            return Redirect(AccesoModel.UrlLogin());
        }
    }
}
