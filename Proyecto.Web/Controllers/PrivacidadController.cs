using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Web.Controllers
{
    public class PrivacidadController : Controller
    {
        // GET: PrivacidadController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PrivacidadController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PrivacidadController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrivacidadController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PrivacidadController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PrivacidadController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PrivacidadController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PrivacidadController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
