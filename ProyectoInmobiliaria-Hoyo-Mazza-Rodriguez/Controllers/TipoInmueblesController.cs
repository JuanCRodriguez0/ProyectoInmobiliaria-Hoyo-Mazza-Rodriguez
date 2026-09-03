using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble repositorioTipoInmueble;

        public TipoInmuebleController(IConfiguration configuration)
        {
            repositorioTipoInmueble = new RepositorioTipoInmueble(configuration);
        }

        // GET: TipoInmueble
        public IActionResult Index()
        {
            var lista = repositorioTipoInmueble.ObtenerTodos();
            return View(lista);
        }

        // GET: TipoInmueble/Details/5
        public IActionResult Details(int id)
        {
            var tipo = repositorioTipoInmueble.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        // GET: TipoInmueble/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoInmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble tipo)
        {
            if (ModelState.IsValid)
            {
                repositorioTipoInmueble.Alta(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        // GET: TipoInmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var tipo = repositorioTipoInmueble.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        // POST: TipoInmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoInmueble tipo)
        {
            if (id != tipo.IdTipoInmueble)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorioTipoInmueble.Modificacion(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        // GET: TipoInmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var tipo = repositorioTipoInmueble.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }

            ViewBag.TieneInmuebles = repositorioTipoInmueble.TieneInmuebles(id);
            return View(tipo);
        }

        // POST: TipoInmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (repositorioTipoInmueble.TieneInmuebles(id))
            {
                var tipo = repositorioTipoInmueble.ObtenerPorId(id);
                ViewBag.TieneInmuebles = true;
                return View("Delete", tipo);
            }

            repositorioTipoInmueble.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}