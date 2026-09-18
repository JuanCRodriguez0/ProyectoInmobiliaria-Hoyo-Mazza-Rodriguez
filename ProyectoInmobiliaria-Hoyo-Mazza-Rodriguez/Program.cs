using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Helpers;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

//ahora en chachos ars
var culturaArgentina = new CultureInfo("es-AR");
CultureInfo.DefaultThreadCurrentCulture = culturaArgentina;
CultureInfo.DefaultThreadCurrentUICulture = culturaArgentina;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    var politica = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(politica));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

var repositorioUsuario = new RepositorioUsuario(app.Configuration);
if (repositorioUsuario.ObtenerTodos().Count == 0)
{
    repositorioUsuario.Alta(new Usuario
    {
        Email = "admin@inmobiliaria.com",
        Clave = PasswordHelper.HashClave("Admin123!"),
        Nombre = "Admin",
        Apellido = "Sistema",
        Rol = "Administrador"
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();