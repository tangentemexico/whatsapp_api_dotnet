using Google.Protobuf;
using Mgk.Base.ControlAcceso;
using Mgk.Base.Parametro;
using Mgk.Base.WebCore.Codigo;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyecto.Web.Controllers
{
    public class ArchivosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    
    }
}
