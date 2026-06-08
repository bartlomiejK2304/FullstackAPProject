using FullstackAPPProject.Data;
using FullstackAPPProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FullstackAPPProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            // Najnowsze 3 ogłoszenia z bazy żeby pokazać je na landing page
            var najnowsze = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Include(o => o.Uzytkownik)
                .OrderByDescending(o => o.DataDodania)
                .Take(3)
                .ToList();

            // Kategorie z liczbą ogłoszeń - do sekcji "Wybierz przedmiot"
            ViewBag.Kategorie = _db.Kategorie
                .Select(k => new {
                    k.Id,
                    k.Nazwa,
                    Ilosc = k.Ogloszenia.Count
                })
                .ToList();

            // Proste statystyki na landing page
            ViewBag.LiczbaOgloszen = _db.Ogloszenia.Count();
            ViewBag.LiczbaKorepetytorow = _db.Uzytkownicy.Count(u => u.Rola == "Korepetytor");
            ViewBag.LiczbaKategorii = _db.Kategorie.Count();

            return View(najnowsze);
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
