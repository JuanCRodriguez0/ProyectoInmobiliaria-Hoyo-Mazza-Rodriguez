using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioTipoInmueble repositorioTipoInmueble;
        private readonly RepositorioInmuebleImagen repositorioInmuebleImagen;
        private readonly IWebHostEnvironment entornoWeb;

        public InmuebleController(IConfiguration configuration, IWebHostEnvironment entornoWeb)
        {
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPropietario = new RepositorioPropietario(configuration);
            repositorioTipoInmueble = new RepositorioTipoInmueble(configuration);
            repositorioInmuebleImagen = new RepositorioInmuebleImagen(configuration);
            this.entornoWeb = entornoWeb;
        }

        private void CargarListas()
        {
            ViewBag.Tipos = repositorioTipoInmueble.ObtenerTodos();
        }

        private string? GuardarArchivo(IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0) return null;

            var carpeta = Path.Combine(entornoWeb.WebRootPath, "images", "inmuebles");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            return $"/images/inmuebles/{nombreArchivo}";
        }

        // GET: Inmueble
        public IActionResult Index(int pagina = 1, string? busqueda = null, bool? disponible = null)
        {
            const int tamanioPagina = 8;
            var resultado = repositorioInmueble.ObtenerPaginado(pagina, tamanioPagina, busqueda, disponible);
            ViewBag.FiltroDisponible = disponible;
            return View(resultado);
        }

        [HttpGet]
        public IActionResult Buscar(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new object[0]);

            var resultado = repositorioInmueble.BuscarPorTexto(term)
                .Select(i => new
                {
                    id = i.IdInmueble,
                    text = $"{i.Direccion} ({i.DescripcionTipo}) - {i.PrecioPorDia:C}/día",
                    precio = i.PrecioPorDia
                });

            return Json(resultado);
        }

        [HttpGet]
        public IActionResult BuscarDisponibles(string term, DateTime? desde, DateTime? hasta)
        {
            if (string.IsNullOrWhiteSpace(term) || desde == null || hasta == null || hasta < desde)
                return Json(new object[0]);

            var resultado = repositorioInmueble.BuscarDisponiblesPorTexto(term, desde.Value, hasta.Value)
                .Select(i => new
                {
                    id = i.IdInmueble,
                    text = $"{i.Direccion} ({i.DescripcionTipo}) - Cupo {i.Cupo} - {i.PrecioPorDia:C}/día",
                    precio = i.PrecioPorDia
                });

            return Json(resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearTipoRapido([FromForm] string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Trim().Length < 3)
                return BadRequest(new { error = "Ingrese una descripción válida (mínimo 3 letras)." });

            if (!System.Text.RegularExpressions.Regex.IsMatch(descripcion.Trim(), @"^[A-Za-zÀ-ÿ\s]+$"))
                return BadRequest(new { error = "La descripción solo puede contener letras." });

            var tipo = new TipoInmueble { Descripcion = descripcion.Trim() };
            repositorioTipoInmueble.Alta(tipo);

            return Json(new { id = tipo.IdTipoInmueble, descripcion = tipo.Descripcion });
        }

        // GET: Inmueble/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            ViewBag.Galeria = repositorioInmuebleImagen.ObtenerPorInmueble(id);
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
        public IActionResult Create(Inmueble inmueble, IFormFile? archivoPortada)
        {
            ModelState.Remove(nameof(Inmueble.Portada));

            if (inmueble.Latitud.HasValue) inmueble.Latitud = Math.Round(inmueble.Latitud.Value, 6);
            if (inmueble.Longitud.HasValue) inmueble.Longitud = Math.Round(inmueble.Longitud.Value, 6);

            if (ModelState.IsValid)
            {
                inmueble.Portada = GuardarArchivo(archivoPortada);
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
            ViewBag.Galeria = repositorioInmuebleImagen.ObtenerPorInmueble(id);
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble, IFormFile? archivoPortada, List<IFormFile>? archivosGaleria)
        {
            if (id != inmueble.IdInmueble)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Inmueble.Portada));

            if (inmueble.Latitud.HasValue) inmueble.Latitud = Math.Round(inmueble.Latitud.Value, 6);
            if (inmueble.Longitud.HasValue) inmueble.Longitud = Math.Round(inmueble.Longitud.Value, 6);

            if (ModelState.IsValid)
            {
                var nuevaPortada = GuardarArchivo(archivoPortada);
                if (nuevaPortada != null)
                {
                    inmueble.Portada = nuevaPortada;
                }
                else
                {
                    var actual = repositorioInmueble.ObtenerPorId(id);
                    inmueble.Portada = actual?.Portada;
                }

                repositorioInmueble.Modificacion(inmueble);

                if (archivosGaleria != null)
                {
                    foreach (var archivo in archivosGaleria)
                    {
                        var ruta = GuardarArchivo(archivo);
                        if (ruta != null)
                        {
                            repositorioInmuebleImagen.Alta(new InmuebleImagen { IdInmueble = id, Ruta = ruta });
                        }
                    }
                }

                return RedirectToAction(nameof(Edit), new { id });
            }
            CargarListas();
            ViewBag.Galeria = repositorioInmuebleImagen.ObtenerPorInmueble(id);
            return View(inmueble);
        }

        // POST: Inmueble/EliminarImagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarImagen(int idImagen, int idInmueble)
        {
            repositorioInmuebleImagen.Baja(idImagen);
            return RedirectToAction(nameof(Edit), new { id = idInmueble });
        }

        // GET: Inmueble/Delete/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
            TempData["Mensaje"] = disponible
                ? "El inmueble vuelve a estar disponible para reservas."
                : "El inmueble quedó suspendido: no va a aparecer para nuevas reservas.";
            return RedirectToAction(nameof(Index));
        }
    }
}