using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class PagoController : Controller
    {
        private readonly RepositorioPago repositorioPago;
        private readonly RepositorioReserva repositorioReserva;

        private const int TAM_PAGINA = 10;

        public PagoController(IConfiguration configuration)
        {
            repositorioPago = new RepositorioPago(configuration);
            repositorioReserva = new RepositorioReserva(configuration);
        }

        private int UsuarioActualId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        
        public IActionResult Index(int idReserva, int pagina = 1)
        {
            var reserva = repositorioReserva.ObtenerPorId(idReserva);
            if (reserva == null)
            {
                return NotFound();
            }

            var total = repositorioPago.ContarPorReserva(idReserva);
            var lista = repositorioPago.ObtenerPorReserva(idReserva, pagina, TAM_PAGINA);

            ViewBag.Reserva = reserva;
            ViewBag.TotalPagado = repositorioPago.TotalPagadoPorReserva(idReserva);
            ViewBag.Pagina = pagina;
            ViewBag.TamPagina = TAM_PAGINA;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)TAM_PAGINA);
            ViewBag.TotalRegistros = total;

            
            ViewBag.NuevoPago = new Pago { IdReserva = idReserva, FechaPago = DateTime.Today };

            return View(lista);
        }

        // GET: Pago/Details/5
        public IActionResult Details(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // GET: Pago/Create?idReserva=5
        public IActionResult Create(int idReserva)
        {
            var reserva = repositorioReserva.ObtenerPorId(idReserva);
            if (reserva == null)
            {
                return NotFound();
            }

            ViewBag.Reserva = reserva;
            return View(new Pago { IdReserva = idReserva, FechaPago = DateTime.Today });
        }

        // POST: Pago/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            var reserva = repositorioReserva.ObtenerPorId(pago.IdReserva);
            if (reserva == null)
            {
                return NotFound();
            }

            if (pago.FechaPago.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(Pago.FechaPago), "La fecha de pago no puede ser futura.");
            }

            if (ModelState.IsValid)
            {
                pago.IdUsuarioCreador = UsuarioActualId;
                repositorioPago.Alta(pago);
                TempData["Mensaje"] = "Pago registrado correctamente.";
                return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
            }

            ViewBag.Reserva = reserva;
            return View(pago);
        }

        // GET: Pago/Edit/5
        public IActionResult Edit(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }

            if (pago.Anulado)
            {
                TempData["Error"] = "No se puede editar un pago anulado.";
                return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
            }

            return View(pago);
        }

        // POST: Pago/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            var original = repositorioPago.ObtenerPorId(id);
            if (original == null)
            {
                return NotFound();
            }

            if (original.Anulado)
            {
                TempData["Error"] = "No se puede editar un pago anulado.";
                return RedirectToAction(nameof(Index), new { idReserva = original.IdReserva });
            }

            if (string.IsNullOrWhiteSpace(pago.Concepto))
            {
                ModelState.AddModelError(nameof(Pago.Concepto), "El concepto es obligatorio");
            }

            // Se ignora cualquier cambio de importe o fecha que venga del formulario
            ModelState.Remove(nameof(Pago.Importe));
            ModelState.Remove(nameof(Pago.FechaPago));

            if (ModelState.IsValid)
            {
                repositorioPago.ModificarConcepto(id, pago.Concepto);
                TempData["Mensaje"] = "Concepto actualizado.";
                return RedirectToAction(nameof(Index), new { idReserva = original.IdReserva });
            }

            original.Concepto = pago.Concepto;
            return View(original);
        }

        // GET: Pago/Anular/5 
        [Authorize(Roles = "Administrador")]
        public IActionResult Anular(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }

            if (pago.Anulado)
            {
                TempData["Error"] = "El pago ya se encuentra anulado.";
                return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
            }

            return View(pago);
        }

        // POST: Pago/Anular/5
        [HttpPost, ActionName("Anular")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult AnularConfirmado(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }

            repositorioPago.Anular(id, UsuarioActualId);
            TempData["Mensaje"] = "Pago anulado.";
            return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
        }
    }
}