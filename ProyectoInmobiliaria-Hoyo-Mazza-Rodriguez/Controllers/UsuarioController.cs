using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Helpers;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly RepositorioUsuario repositorioUsuario;

        public UsuarioController(IConfiguration configuration)
        {
            repositorioUsuario = new RepositorioUsuario(configuration);
        }

        private int UsuarioActualId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View(repositorioUsuario.ObtenerTodos());
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Usuario usuario)
        {
            if (repositorioUsuario.ExisteEmail(usuario.Email))
                ModelState.AddModelError(nameof(Usuario.Email), "Ya existe un usuario con ese email");

            if (string.IsNullOrWhiteSpace(usuario.ClaveNueva))
                ModelState.AddModelError(nameof(Usuario.ClaveNueva), "Debe indicar una contraseña");

            ModelState.Remove(nameof(Usuario.Clave));

            if (ModelState.IsValid)
            {
                usuario.Clave = PasswordHelper.HashClave(usuario.ClaveNueva!);
                repositorioUsuario.Alta(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario) return NotFound();
            ModelState.Remove(nameof(Usuario.Clave));
            ModelState.Remove(nameof(Usuario.ClaveNueva));

            if (ModelState.IsValid)
            {
                repositorioUsuario.Modificacion(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var usuario = repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioUsuario.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Perfil()
        {
            var usuario = repositorioUsuario.ObtenerPorId(UsuarioActualId);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Perfil(Usuario usuario)
        {
            usuario.IdUsuario = UsuarioActualId;
            ModelState.Remove(nameof(Usuario.Clave));
            ModelState.Remove(nameof(Usuario.Rol));
            ModelState.Remove(nameof(Usuario.Email));

            var actual = repositorioUsuario.ObtenerPorId(UsuarioActualId)!;
            usuario.Rol = actual.Rol; // el empleado no puede cambiarse el rol a sí mismo

            if (ModelState.IsValid)
            {
                repositorioUsuario.Modificacion(usuario);

                if (!string.IsNullOrWhiteSpace(usuario.ClaveNueva))
                {
                    repositorioUsuario.CambiarClave(UsuarioActualId, PasswordHelper.HashClave(usuario.ClaveNueva));
                }

                TempData["Mensaje"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Perfil));
            }
            return View(usuario);
        }
    }
}