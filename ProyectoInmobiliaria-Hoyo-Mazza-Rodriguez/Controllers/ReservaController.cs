using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class ReservaController : Controller
    {
        private readonly RepositorioReserva repositorioReserva;
        private readonly RepositorioInquilino repositorioInquilino;
        private readonly RepositorioInmueble repositorioInmueble;

        public ReservaController(IConfiguration configuration)
        {
            repositorioReserva = new RepositorioReserva(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
        }

        private void CargarListas()
        {
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos()
                .Select(i => new SelectListItem
                {
                    Value = i.IdInquilino.ToString(),
                    Text = $"{i.Dni} - {i.Nombre} {i.Apellido}"
                })
                .ToList();

            var inmuebles = repositorioInmueble.ObtenerTodos();

            ViewBag.InmueblesConPrecio = inmuebles;

            ViewBag.Inmuebles = inmuebles
                .Select(i => new SelectListItem
                {
                    Value = i.IdInmueble.ToString(),
                    Text = $"{i.Direccion} ({i.DescripcionTipo}) - {i.PrecioPorDia:C}/día"
                })
                .ToList();
        }

        // GET: Reserva
        public IActionResult Index()
        {
            var lista = repositorioReserva.ObtenerTodos();
            return View(lista);
        }

        // GET: Reserva/Details/5
        public IActionResult Details(int id)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // GET: Reserva/Create
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        // POST: Reserva/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            var inmuebleSeleccionado = repositorioInmueble.ObtenerPorId(reserva.IdInmueble);
            if (inmuebleSeleccionado != null)
            {
                reserva.MontoPorDia = inmuebleSeleccionado.PrecioPorDia;
                ModelState.Remove(nameof(Reserva.MontoPorDia));
            }

            ValidarReserva(reserva);

            if (ModelState.IsValid)
            {
                repositorioReserva.Alta(reserva);
                return RedirectToAction(nameof(Index));
            }
            CargarListas();
            return View(reserva);
        }

        // GET: Reserva/Edit/5
        public IActionResult Edit(int id)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            CargarListas();
            return View(reserva);
        }

        // POST: Reserva/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva)
        {
            if (id != reserva.IdReserva)
            {
                return NotFound();
            }

            ValidarReserva(reserva);

            if (ModelState.IsValid)
            {
                repositorioReserva.Modificacion(reserva);
                return RedirectToAction(nameof(Index));
            }
            CargarListas();
            return View(reserva);
        }

        // GET: Reserva/Delete/5
        public IActionResult Delete(int id)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioReserva.Baja(id);
            return RedirectToAction(nameof(Index));
        }
        private void ValidarReserva(Reserva reserva)
        {
            if (reserva.FechaHasta <= reserva.FechaDesde)
            {
                ModelState.AddModelError(nameof(Reserva.FechaHasta), "La fecha hasta debe ser posterior a la fecha desde");
                return;
            }

            bool ocupado = repositorioReserva.ExisteSolapamiento(
                reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta, reserva.IdReserva);

            if (ocupado)
            {
                ModelState.AddModelError(nameof(Reserva.IdInmueble), "El inmueble ya se encuentra reservado en esas fechas");
            }
        }
    }
}
