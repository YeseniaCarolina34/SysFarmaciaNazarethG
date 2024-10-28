using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysFarmaciaNazarethG.Models;
using System.Diagnostics;

namespace SysFarmaciaNazarethG.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        // Acci�n p�blica que muestra la p�gina de inicio de la cafeter�a
        [AllowAnonymous]
        public IActionResult Inicio()
        {
            return View(); // Esta vista ser� accesible sin necesidad de autenticaci�n
        }


        // Acci�n principal que redirige al login
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}