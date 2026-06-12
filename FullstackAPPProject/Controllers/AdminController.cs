using FullstackAPPProject.Data;
using FullstackAPPProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullstackAPPProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.LiczbaUzytkownikow = _db.Uzytkownicy.Count();
            ViewBag.LiczbaOgloszen = _db.Ogloszenia.Count();
            ViewBag.LiczbaKategorii = _db.Kategorie.Count();
            ViewBag.LiczbaWiadomosci = _db.Wiadomosci.Count();
            ViewBag.LiczbaOpinii = _db.Opinie.Count();
            ViewBag.LiczbaUsuniec = _db.UsunieciaKont.Count();
            return View();
        }


        public IActionResult Uzytkownicy()
        {
            var lista = _db.Uzytkownicy
                .OrderBy(u => u.Nazwa)
                .ToList();
            return View(lista);
        }

        [HttpPost]
        public IActionResult UsunUzytkownika(int id)
        {
            var uzytkownik = _db.Uzytkownicy.Find(id);
            if (uzytkownik == null) return NotFound();

            if (uzytkownik.Rola == "Admin")
            {
                TempData["Blad"] = "Nie można usunąć konta administratora!";
                return RedirectToAction("Uzytkownicy");
            }

            var mojeId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            if (id == mojeId)
            {
                TempData["Blad"] = "Nie możesz usunąć samego siebie!";
                return RedirectToAction("Uzytkownicy");
            }

            var wiadomosci = _db.Wiadomosci.Where(w => w.NadawcaId == id || w.OdbiorcaId == id);
            _db.Wiadomosci.RemoveRange(wiadomosci);

            var opinie = _db.Opinie.Where(o => o.AutorId == id || o.KorepetytorId == id);
            _db.Opinie.RemoveRange(opinie);

            var ogloszenia = _db.Ogloszenia.Where(o => o.UzytkownikId == id);
            _db.Ogloszenia.RemoveRange(ogloszenia);

            _db.Uzytkownicy.Remove(uzytkownik);
            _db.SaveChanges();

            TempData["Sukces"] = "Użytkownik " + uzytkownik.Nazwa + " został usunięty.";
            return RedirectToAction("Uzytkownicy");
        }


        public IActionResult Kategorie()
        {
            var lista = _db.Kategorie
                .Include(k => k.Ogloszenia)
                .OrderBy(k => k.Nazwa)
                .ToList();
            return View(lista);
        }

        [HttpPost]
        public IActionResult DodajKategorie(string nazwa)
        {
            if (string.IsNullOrWhiteSpace(nazwa))
            {
                TempData["Blad"] = "Nazwa kategorii nie może być pusta.";
                return RedirectToAction("Kategorie");
            }

            if (_db.Kategorie.Any(k => k.Nazwa == nazwa.Trim()))
            {
                TempData["Blad"] = "Kategoria o tej nazwie już istnieje.";
                return RedirectToAction("Kategorie");
            }

            _db.Kategorie.Add(new Kategoria { Nazwa = nazwa.Trim() });
            _db.SaveChanges();

            TempData["Sukces"] = "Dodano kategorię: " + nazwa.Trim();
            return RedirectToAction("Kategorie");
        }
        [HttpPost]
        public IActionResult UsunKategorie(int id)
        {
            var kategoria = _db.Kategorie.Include(k => k.Ogloszenia).FirstOrDefault(k => k.Id == id);
            if (kategoria == null) return NotFound();

            if (kategoria.Ogloszenia.Any())
            {
                TempData["Blad"] = "Nie można usunąć kategorii, która ma przypisane ogłoszenia (" + kategoria.Ogloszenia.Count + ").";
                return RedirectToAction("Kategorie");
            }

            _db.Kategorie.Remove(kategoria);
            _db.SaveChanges();

            TempData["Sukces"] = "Usunięto kategorię: " + kategoria.Nazwa;
            return RedirectToAction("Kategorie");
        }


        public IActionResult UsunieciaKont()
        {
            var lista = _db.UsunieciaKont
                .OrderByDescending(u => u.DataUsuniecia)
                .ToList();
            return View(lista);
        }


        public IActionResult Ogloszenia()
        {
            var lista = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Include(o => o.Uzytkownik)
                .OrderByDescending(o => o.DataDodania)
                .ToList();
            return View(lista);
        }

        [HttpPost]
        public IActionResult UsunOgloszenie(int id)
        {
            var ogloszenie = _db.Ogloszenia.Find(id);
            if (ogloszenie == null) return NotFound();

            var wiadomosci = _db.Wiadomosci.Where(w => w.OgloszenieId == id);
            _db.Wiadomosci.RemoveRange(wiadomosci);

            _db.Ogloszenia.Remove(ogloszenie);
            _db.SaveChanges();

            TempData["Sukces"] = "Ogłoszenie zostało usunięte.";
            return RedirectToAction("Ogloszenia");
        }
    }
}
