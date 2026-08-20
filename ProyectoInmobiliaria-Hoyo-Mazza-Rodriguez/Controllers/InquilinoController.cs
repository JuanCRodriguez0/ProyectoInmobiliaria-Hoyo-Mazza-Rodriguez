using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly RepositorioInquilino repositorioInquilino;

        public InquilinosController(IConfiguration configuration)
        {
            repositorioInquilino = new RepositorioInquilino(configuration);
        }

        // GET: Inquilinos
        public IActionResult Index()
        {
            var lista = repositorioInquilino.ObtenerTodos();
            return View(lista);
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
        public IActionResult Create(Inquilinos inquilino)
        {
            if (ModelState.IsValid)
            {
                repositorioInquilino.Alta(inquilino);
                return RedirectToAction(nameof(Index));
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
        public IActionResult Edit(int id, Inquilinos inquilino)
        {
            if (id != inquilino.IdInquilino)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorioInquilino.Modificacion(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Delete/5
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
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioInquilino.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}