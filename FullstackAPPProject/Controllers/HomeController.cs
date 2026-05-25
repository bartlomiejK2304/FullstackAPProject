using FullstackAPPProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FullstackAPPProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            var testoweOgloszenia = new List<Ogloszenie>
        {

        new Ogloszenie {
                Id = 1,
                Tytul = "Korepetycje z Matematyki",
                Miasto = "Warszawa",
                Cena = 60,
                Kategoria = new Kategoria { Nazwa = "Matematyka" } 
            },

            new Ogloszenie {
                Id = 2,
                Tytul = "Angielski dla początkujących",
                Miasto = "Kraków",
                Cena = 50,
                Kategoria = new Kategoria { Nazwa = "Języki obce" }
            }
        };

            
            return View(testoweOgloszenia);
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
