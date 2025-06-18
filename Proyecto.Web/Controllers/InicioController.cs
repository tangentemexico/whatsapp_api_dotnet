using Mgk.Base.ControlAcceso;
using Mgk.Base.Parametro;
using Mgk.Base.WebCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mgk.Base.WebCore.Controllers
{
    public class InicioController : Controller
    {
        private readonly ILogger<InicioController> _logger;

        public InicioController(ILogger<InicioController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(AppConstantes.SESION_NOMBRE) == null)
                return Redirect(AccesoModel.UrlLogin());

            return View();
        }

        public IActionResult NuevoPassword(String token)
        {
            ViewBag.token = token;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}