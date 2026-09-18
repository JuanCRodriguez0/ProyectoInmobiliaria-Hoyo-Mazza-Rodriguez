using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino repositorioInquilino;

        public InquilinoController(IConfiguration configuration)
        {
            repositorioInquilino = new RepositorioInquilino(configuration);
        }

        // GET: Inquilinos
        public IActionResult Index(int pagina = 1, string? busqueda = null)
        {
            const int tamanioPagina = 10;
            var resultado = repositorioInquilino.ObtenerPaginado(pagina, tamanioPagina, busqueda);
            return View(resultado);
        }

        [HttpGet]
        public IActionResult Buscar(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new object[0]);

            var resultado = repositorioInquilino.BuscarPorTexto(term)
                .Select(i => new { id = i.IdInquilino, text = $"{i.Dni} - {i.Nombre} {i.Apellido}" });

            return Json(resultado);
        }

        // GET: Inquilinos/Details/5
        public IActionResult Details(int id)
        {
            var inquilino = repositorioInquilino.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (repositorioInquilino.ExisteDni(inquilino.Dni))
                ModelState.AddModelError(nameof(Inquilino.Dni), "Ya existe un inquilino registrado con ese DNI.");

            if (ModelState.IsValid)
            {
                try
                {
                    repositorioInquilino.Alta(inquilino);
                    return RedirectToAction(nameof(Index));
                }
                catch (MySqlException)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo guardar el inquilino (dato duplicado). Verifique el DNI.");
                }
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Edit/5
        public IActionResult Edit(int id)
        {
            var inquilino = repositorioInquilino.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            if (id != inquilino.IdInquilino)
            {
                return NotFound();
            }

            if (repositorioInquilino.ExisteDni(inquilino.Dni, inquilino.IdInquilino))
                ModelState.AddModelError(nameof(Inquilino.Dni), "Ya existe otro inquilino registrado con ese DNI.");

            if (ModelState.IsValid)
            {
                try
                {
                    repositorioInquilino.Modificacion(inquilino);
                    return RedirectToAction(nameof(Index));
                }
                catch (MySqlException)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo guardar el inquilino (dato duplicado). Verifique el DNI.");
                }
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Delete/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var inquilino = repositorioInquilino.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioInquilino.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}