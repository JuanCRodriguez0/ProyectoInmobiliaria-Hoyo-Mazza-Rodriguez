using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class InformesController : Controller
    {
        private const int TAM_PAGINA = 10;

        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioReserva repositorioReserva;
        private readonly RepositorioPropietario repositorioPropietario;

        public InformesController(IConfiguration configuration)
        {
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioReserva = new RepositorioReserva(configuration);
            repositorioPropietario = new RepositorioPropietario(configuration);
        }

        // GET: Informes
        public IActionResult Index()
        {
            return View();
        }

        // GET: Informes/PorPropietario?idPropietario=5&pagina=1
        public IActionResult PorPropietario(int? idPropietario, int pagina = 1)
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.IdPropietarioSeleccionado = idPropietario;

            var resultado = idPropietario.HasValue
                ? repositorioInmueble.ObtenerPorPropietario(idPropietario.Value, pagina, TAM_PAGINA)
                : new PaginadoResultado<Inmueble>();

            return View(resultado);
        }

        // GET: Informes/MasReservados?dias=365&pagina=1
        public IActionResult MasReservados(int dias = 365, int pagina = 1)
        {
            ViewBag.Dias = dias;
            var resultado = repositorioInmueble.ObtenerMasReservados(dias, pagina, TAM_PAGINA);
            return View(resultado);
        }

        // GET: Informes/SinReservas?dias=30&pagina=1
        public IActionResult SinReservas(int dias = 30, int pagina = 1)
        {
            ViewBag.Dias = dias;
            var resultado = repositorioInmueble.ObtenerSinReservasEn(dias, pagina, TAM_PAGINA);
            return View(resultado);
        }

        // GET: Informes/ReservasVigentes?pagina=1
        public IActionResult ReservasVigentes(int pagina = 1)
        {
            var resultado = repositorioReserva.ObtenerVigentes(pagina, TAM_PAGINA);
            return View(resultado);
        }

        // GET: Informes/ReservasPorTerminar?dias=7&pagina=1
        public IActionResult ReservasPorTerminar(int dias = 7, int pagina = 1)
        {
            ViewBag.Dias = dias;
            var resultado = repositorioReserva.ObtenerQueTerminanEn(dias, pagina, TAM_PAGINA);
            return View(resultado);
        }

        // GET: Informes/Disponibilidad?desde=2026-01-01&hasta=2026-01-15&pagina=1
        public IActionResult Disponibilidad(DateTime? desde, DateTime? hasta, int pagina = 1)
        {
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;

            var resultado = new PaginadoResultado<Inmueble>();

            if (desde.HasValue && hasta.HasValue && hasta.Value >= desde.Value)
            {
                resultado = repositorioInmueble.ObtenerDisponiblesEntreFechas(desde.Value, hasta.Value, pagina, TAM_PAGINA);
            }
            else if (desde.HasValue || hasta.HasValue)
            {
                ViewBag.Error = "Ingresá una fecha desde y una fecha hasta válidas (hasta no puede ser anterior a desde).";
            }

            return View(resultado);
        }
    }
}