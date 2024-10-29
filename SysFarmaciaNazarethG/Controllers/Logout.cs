using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public class LogoutController : Controller
{
    // Método para cerrar sesión
    public async Task<IActionResult> Index()
    {
        // Cierra la sesión del usuario actual
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirige al usuario a la página de inicio de sesión o de inicio
        return RedirectToAction("Login", "Usuario");  // Puedes cambiar "Login" a la acción que prefieras
    }
}
