using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;
using SysFarmaciaNazarethG.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BDContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Registrar los servicios necesarios

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser() // Exige que el usuario esté autenticado
                    .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy)); // Aplica la política a todas las páginas por defecto
});


builder.Services.AddScoped<UsuarioService>();

// Agregar soporte de memoria distribuida para sesiones
builder.Services.AddDistributedMemoryCache();

// Configurar las opciones de la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Ajusta el tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true;                 // Asegura que solo se pueda acceder a la cookie desde HTTP
    options.Cookie.IsEssential = true;              // Marca la cookie como esencial
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
       options.LoginPath = "/Usuario/Login"; // Página de inicio de sesión
       options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración de la cookie
       options.AccessDeniedPath = "/Usuario/AccessDenied"; // Página de acceso denegado
       
        
   });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Cliente", policy => policy.RequireRole("Cliente"));
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

var app = builder.Build();

// Configurar el middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

// Habilitar las sesiones y autenticación
app.UseSession();
app.UseAuthentication(); // Debe ir antes de UseAuthorization
app.UseAuthorization();

// Configurar las rutas de los controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Inicio}/{id?}");


app.Run();