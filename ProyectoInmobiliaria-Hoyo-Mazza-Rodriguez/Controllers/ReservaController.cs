using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        private int UsuarioActualId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: Reserva
        public IActionResult Index(int pagina = 1, string? busqueda = null)
        {
            const int tamanioPagina = 10;
            var resultado = repositorioReserva.ObtenerPaginado(pagina, tamanioPagina, busqueda);
            return View(resultado);
        }

        // GET: Reserva/Details/5
        public IActionResult Details(int id)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewBag.EsAdministrador = User.IsInRole("Administrador");
            return View(reserva);
        }

        // GET: Reserva/Create
        public IActionResult Create()
        {
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

                if (!inmuebleSeleccionado.Disponible)
                {
                    ModelState.AddModelError(nameof(Reserva.IdInmueble),
                        "Ese inmueble está suspendido por el propietario y no puede reservarse.");
                }
            }

            ValidarReserva(reserva);

            if (ModelState.IsValid)
            {
                try
                {
                    repositorioReserva.Alta(reserva, UsuarioActualId);
                    TempData["Mensaje"] = "Reserva creada correctamente.";
                    return RedirectToAction(nameof(Details), new { id = reserva.IdReserva });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
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

            if (reserva.Terminada)
            {
                TempData["Error"] = "No se puede editar una reserva que ya fue terminada anticipadamente.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(reserva);
        }

        // POST: Reserva/Edit/5
        // Solo modif fechas y monto; inquilino e inmueble fijos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva)
        {
            if (id != reserva.IdReserva)
            {
                return NotFound();
            }

            var original = repositorioReserva.ObtenerPorId(id);
            if (original == null)
            {
                return NotFound();
            }

            if (original.Terminada)
            {
                TempData["Error"] = "No se puede editar una reserva que ya fue terminada anticipadamente.";
                return RedirectToAction(nameof(Details), new { id });
            }

            reserva.IdInquilino = original.IdInquilino;
            reserva.IdInmueble = original.IdInmueble;
            ModelState.Remove(nameof(Reserva.IdInquilino));
            ModelState.Remove(nameof(Reserva.IdInmueble));

            ValidarReserva(reserva);

            if (ModelState.IsValid)
            {
                try
                {
                    repositorioReserva.Modificacion(reserva);
                    TempData["Mensaje"] = "Reserva actualizada correctamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            reserva.NombreInquilino = original.NombreInquilino;
            reserva.DireccionInmueble = original.DireccionInmueble;
            return View(reserva);
        }

        // GET: Reserva/Delete/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioReserva.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Terminar(int id, DateTime? fechaEfectiva)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }

            if (reserva.Terminada)
            {
                TempData["Error"] = "Esta reserva ya fue terminada anticipadamente.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var fecha = fechaEfectiva?.Date ?? DateTime.Today;

            if (fecha < reserva.FechaDesde.Date) fecha = reserva.FechaDesde.Date;
            if (fecha >= reserva.FechaHastaOriginal.Date) fecha = reserva.FechaHastaOriginal.Date.AddDays(-1);

            ViewBag.FechaEfectiva = fecha;
            ViewBag.MultaCalculada = repositorioReserva.CalcularMulta(reserva, fecha);

            return View(reserva);
        }

        [HttpPost, ActionName("Terminar")]
        [ValidateAntiForgeryToken]
        public IActionResult TerminarConfirmado(int id, DateTime fechaEfectiva, bool confirmaPago)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }

            if (!confirmaPago)
            {
                ModelState.AddModelError(string.Empty,
                    "Debe confirmar el pago de la multa para poder finalizar la reserva.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var multa = repositorioReserva.TerminarAnticipadamente(id, fechaEfectiva, UsuarioActualId);
                    TempData["Mensaje"] = $"Reserva terminada. Se registró una multa de {multa:C}.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.FechaEfectiva = fechaEfectiva.Date;
            ViewBag.MultaCalculada = repositorioReserva.CalcularMulta(reserva, fechaEfectiva.Date);
            return View(reserva);
        }

        // GET: Reserva/Renovar/5
        public IActionResult Renovar(int id)
        {
            var original = repositorioReserva.ObtenerPorId(id);
            if (original == null)
            {
                return NotFound();
            }

            var inmueble = repositorioInmueble.ObtenerPorId(original.IdInmueble);
            var nuevaFechaDesde = original.FechaHasta.Date.AddDays(1);

            var propuesta = new Reserva
            {
                IdInquilino = original.IdInquilino,
                IdInmueble = original.IdInmueble,
                MontoPorDia = inmueble?.PrecioPorDia ?? original.MontoPorDia,
                FechaDesde = nuevaFechaDesde,
                FechaHasta = nuevaFechaDesde.AddMonths(1),
                IdReservaOrigen = original.IdReserva
            };

            ViewBag.ReservaOrigen = original;
            return View(propuesta);
        }

        // POST: Reserva/Renovar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Renovar(int id, Reserva reserva)
        {
            var original = repositorioReserva.ObtenerPorId(id);
            if (original == null)
            {
                return NotFound();
            }

            reserva.IdInquilino = original.IdInquilino;
            reserva.IdInmueble = original.IdInmueble;
            reserva.IdReservaOrigen = original.IdReserva;
            ModelState.Remove(nameof(Reserva.IdInquilino));
            ModelState.Remove(nameof(Reserva.IdInmueble));
            ModelState.Remove(nameof(Reserva.IdReservaOrigen));

            ValidarReserva(reserva);

            if (ModelState.IsValid)
            {
                try
                {
                    repositorioReserva.Alta(reserva, UsuarioActualId);
                    TempData["Mensaje"] = "Reserva renovada correctamente.";
                    return RedirectToAction(nameof(Details), new { id = reserva.IdReserva });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.ReservaOrigen = original;
            return View(reserva);
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