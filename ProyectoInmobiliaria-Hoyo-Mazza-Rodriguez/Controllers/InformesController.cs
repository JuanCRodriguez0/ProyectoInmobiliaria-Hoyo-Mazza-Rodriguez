using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class InformesController : Controller
    {
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

        // GET: Informes/PorPropietario?idPropietario=5
        public IActionResult PorPropietario(int? idPropietario)
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.IdPropietarioSeleccionado = idPropietario;

            var lista = idPropietario.HasValue
                ? repositorioInmueble.ObtenerPorPropietario(idPropietario.Value)
                : new List<Inmueble>();

            return View(lista);
        }

        // GET: Informes/MasReservados?dias=365&top=10
        public IActionResult MasReservados(int dias = 365, int top = 10)
        {
            ViewBag.Dias = dias;
            ViewBag.Top = top;
            var lista = repositorioInmueble.ObtenerMasReservados(dias, top);
            return View(lista);
        }

        // GET: Informes/SinReservas?dias=30
        public IActionResult SinReservas(int dias = 30)
        {
            ViewBag.Dias = dias;
            var lista = repositorioInmueble.ObtenerSinReservasEn(dias);
            return View(lista);
        }

        // GET: Informes/ReservasVigentes
        public IActionResult ReservasVigentes()
        {
            var lista = repositorioReserva.ObtenerVigentes();
            return View(lista);
        }

        // GET: Informes/ReservasPorTerminar?dias=7
        public IActionResult ReservasPorTerminar(int dias = 7)
        {
            ViewBag.Dias = dias;
            var lista = repositorioReserva.ObtenerQueTerminanEn(dias);
            return View(lista);
        }

        // GET: Informes/Disponibilidad?desde=2026-01-01&hasta=2026-01-15
        public IActionResult Disponibilidad(DateTime? desde, DateTime? hasta)
        {
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;

            var lista = new List<Inmueble>();

            if (desde.HasValue && hasta.HasValue && hasta.Value >= desde.Value)
            {
                lista = repositorioInmueble.ObtenerDisponiblesEntreFechas(desde.Value, hasta.Value);
            }
            else if (desde.HasValue || hasta.HasValue)
            {
                ViewBag.Error = "Ingresá una fecha desde y una fecha hasta válidas (hasta no puede ser anterior a desde).";
            }

            return View(lista);
        }
    }
}