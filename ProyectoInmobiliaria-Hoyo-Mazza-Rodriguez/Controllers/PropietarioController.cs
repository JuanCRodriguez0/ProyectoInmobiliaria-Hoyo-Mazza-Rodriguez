using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario repositorioPropietario;

        public PropietarioController(IConfiguration configuration)
        {
            repositorioPropietario = new RepositorioPropietario(configuration);
        }

        // GET: Propietario
        public IActionResult Index(int pagina = 1, string? busqueda = null)
        {
            const int tamanioPagina = 10;
            var resultado = repositorioPropietario.ObtenerPaginado(pagina, tamanioPagina, busqueda);
            return View(resultado);
        }

        [HttpGet]
        public IActionResult Buscar(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new object[0]);

            var resultado = repositorioPropietario.BuscarPorTexto(term)
                .Select(p => new { id = p.IdPropietario, text = $"{p.Dni} - {p.Nombre} {p.Apellido}" });

            return Json(resultado);
        }

        // GET: Propietario/Details/5
        public IActionResult Details(int id)
        {
            var propietario = repositorioPropietario.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // GET: Propietario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repositorioPropietario.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // GET: Propietario/Edit/5
        public IActionResult Edit(int id)
        {
            var propietario = repositorioPropietario.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (id != propietario.IdPropietario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorioPropietario.Modificacion(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // GET: Propietario/Delete/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var propietario = repositorioPropietario.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View("Delete", propietario);
        }

        // POST: Propietario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioPropietario.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}