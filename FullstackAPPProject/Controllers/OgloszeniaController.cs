using FullstackAPPProject.Data;
using FullstackAPPProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FullstackAPPProject.Controllers
{
    public class OgloszeniaController : Controller
    {
        private readonly AppDbContext _db;

        public OgloszeniaController(AppDbContext db)
        {
            _db = db;
        }

        // Lista wszystkich ogłoszeń + proste wyszukiwanie po tytule i kategorii
        public IActionResult Index(string? szukaj, int? kategoriaId)
        {
            // Pobieramy ogłoszenia razem z kategorią i autorem
            var lista = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Include(o => o.Uzytkownik)
                .AsQueryable();

            // Filtrowanie po nazwie (jeśli wpisano)
            if (!string.IsNullOrEmpty(szukaj))
            {
                lista = lista.Where(o => o.Tytul.Contains(szukaj));
            }

            // Filtrowanie po kategorii (jeśli wybrano)
            if (kategoriaId.HasValue)
            {
                lista = lista.Where(o => o.KategoriaId == kategoriaId.Value);
            }

            // Sortujemy od najnowszych
            var wynik = lista.OrderByDescending(o => o.DataDodania).ToList();

            // Lista kategorii do dropdownu w widoku
            ViewBag.Kategorie = _db.Kategorie.ToList();
            ViewBag.Szukaj = szukaj;
            ViewBag.WybranaKategoria = kategoriaId;

            return View(wynik);
        }

        // Szczegóły jednego ogłoszenia
        public IActionResult Szczegoly(int id)
        {
            var ogloszenie = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Include(o => o.Uzytkownik)
                .FirstOrDefault(o => o.Id == id);

            if (ogloszenie == null)
                return NotFound();

            return View(ogloszenie);
        }

        // Dodawanie nowego ogłoszenia - GET (formularz)
        [Authorize]
        [HttpGet]
        public IActionResult Dodaj()
        {
            WstawKategorieDoViewBag();
            return View(new Ogloszenie());
        }

        // Dodawanie - POST (zapis do bazy)
        [Authorize]
        [HttpPost]
        public IActionResult Dodaj(Ogloszenie model)
        {
           
            ModelState.Remove("Uzytkownik");
            ModelState.Remove("Kategoria");

            if (!ModelState.IsValid)
            {
                WstawKategorieDoViewBag();
                return View(model);
            }

            
            model.UzytkownikId = PobierzIdZalogowanego();
            model.DataDodania = DateTime.Now;

            _db.Ogloszenia.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Szczegoly", new { id = model.Id });
        }

       
        [Authorize]
        [HttpGet]
        public IActionResult Edytuj(int id)
        {
            var ogloszenie = _db.Ogloszenia.Find(id);
            if (ogloszenie == null)
                return NotFound();

            if (ogloszenie.UzytkownikId != PobierzIdZalogowanego())
                return Forbid();

            WstawKategorieDoViewBag();
            return View(ogloszenie);
        }

        
        [Authorize]
        [HttpPost]
        public IActionResult Edytuj(Ogloszenie model)
        {
            ModelState.Remove("Uzytkownik");
            ModelState.Remove("Kategoria");

            if (!ModelState.IsValid)
            {
                WstawKategorieDoViewBag();
                return View(model);
            }

            var ogloszenie = _db.Ogloszenia.Find(model.Id);
            if (ogloszenie == null)
                return NotFound();

            // Sprawdzamy czy to autor
            if (ogloszenie.UzytkownikId != PobierzIdZalogowanego())
                return Forbid();

            // Aktualizujemy pola
            ogloszenie.Tytul = model.Tytul;
            ogloszenie.Opis = model.Opis;
            ogloszenie.Cena = model.Cena;
            ogloszenie.Miasto = model.Miasto;
            ogloszenie.Forma = model.Forma;
            ogloszenie.KategoriaId = model.KategoriaId;

            _db.SaveChanges();
            return RedirectToAction("Szczegoly", new { id = ogloszenie.Id });
        }

        // Usuwanie ogłoszenia
        [Authorize]
        [HttpPost]
        public IActionResult Usun(int id)
        {
            var ogloszenie = _db.Ogloszenia.Find(id);
            if (ogloszenie == null)
                return NotFound();

            if (ogloszenie.UzytkownikId != PobierzIdZalogowanego())
                return Forbid();

            _db.Ogloszenia.Remove(ogloszenie);
            _db.SaveChanges();
            return RedirectToAction("Moje");
        }

        
        [Authorize]
        public IActionResult Moje()
        {
            int idUzytkownika = PobierzIdZalogowanego();
            var moje = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Where(o => o.UzytkownikId == idUzytkownika)
                .OrderByDescending(o => o.DataDodania)
                .ToList();

            return View(moje);
        }

        
        private int PobierzIdZalogowanego()
        {
            var idText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idText!);
        }

        // Wstawia kategorie do ViewBag (do listy rozwijanej w formularzu)
        private void WstawKategorieDoViewBag()
        {
            var kategorie = _db.Kategorie.OrderBy(k => k.Nazwa).ToList();
            ViewBag.Kategorie = new SelectList(kategorie, "Id", "Nazwa");
        }
    }

}
