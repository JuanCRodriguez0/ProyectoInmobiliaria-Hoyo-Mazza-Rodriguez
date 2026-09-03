using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioTipoInmueble repositorioTipoInmueble;

        public InmuebleController(IConfiguration configuration)
        {
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPropietario = new RepositorioPropietario(configuration);
            repositorioTipoInmueble = new RepositorioTipoInmueble(configuration);
        }

        private void CargarListas()
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.Tipos = repositorioTipoInmueble.ObtenerTodos();
        }

        // GET: Inmueble
        public IActionResult Index()
        {
            var lista = repositorioInmueble.ObtenerTodos();
            return View(lista);
        }

        // GET: Inmueble/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repositorioInmueble.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }
            CargarListas();
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            CargarListas();
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.IdInmueble)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorioInmueble.Modificacion(inmueble);
                return RedirectToAction(nameof(Index));
            }
            CargarListas();
            return View(inmueble);
        }

        // GET: Inmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View("Delete", inmueble);
        }

        // POST: Inmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioInmueble.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Inmueble/Suspender/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Suspender(int id, bool disponible)
        {
            repositorioInmueble.CambiarDisponibilidad(id, disponible);
            return RedirectToAction(nameof(Index));
        }
    }
}