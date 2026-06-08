using FullstackAPPProject.Data;
using FullstackAPPProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FullstackAPPProject.Controllers
{
    [Authorize] 
    public class WiadomosciController : Controller
    {
        private readonly AppDbContext _db;

        public WiadomosciController(AppDbContext db)
        {
            _db = db;
        }

        // Lista wiadomosci z uzytkownikiem
        public IActionResult Index()
        {
            int mojeId = PobierzIdZalogowanego();

            // œci¹gniêcie wiadomosci 
            var moje = _db.Wiadomosci
                .Include(w => w.Ogloszenie)
                    .ThenInclude(o => o!.Kategoria)
                .Include(w => w.Nadawca)
                .Include(w => w.Odbiorca)
                .Where(w => w.NadawcaId == mojeId || w.OdbiorcaId == mojeId)
                .ToList();

            var konwersacje = moje
                .GroupBy(w => new
                {
                    w.OgloszenieId,
                    InnyId = w.NadawcaId == mojeId ? w.OdbiorcaId : w.NadawcaId
                })
                .Select(g => new KonwersacjaPodglad
                {
                    OgloszenieId = g.Key.OgloszenieId,
                    InnyId = g.Key.InnyId,
                    Ogloszenie = g.First().Ogloszenie!,
                    Inny = g.First().NadawcaId == mojeId
                        ? g.First().Odbiorca!
                        : g.First().Nadawca!,
                    OstatniaWiadomosc = g.OrderByDescending(w => w.DataWyslania).First(),
                    Nieprzeczytane = g.Count(w => w.OdbiorcaId == mojeId && !w.Przeczytana)
                })
                .OrderByDescending(k => k.OstatniaWiadomosc.DataWyslania)
                .ToList();

            return View(konwersacje);
        }

        // Widok jednej konwersacji z odpowiedziami
        public IActionResult Konwersacja(int ogloszenieId, int innyId)
        {
            int mojeId = PobierzIdZalogowanego();

            var ogloszenie = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .FirstOrDefault(o => o.Id == ogloszenieId);
            if (ogloszenie == null) return NotFound();

            var inny = _db.Uzytkownicy.Find(innyId);
            if (inny == null) return NotFound();

            var wiadomosci = _db.Wiadomosci
                .Include(w => w.Nadawca)
                .Where(w => w.OgloszenieId == ogloszenieId &&
                            ((w.NadawcaId == mojeId && w.OdbiorcaId == innyId) ||
                             (w.NadawcaId == innyId && w.OdbiorcaId == mojeId)))
                .OrderBy(w => w.DataWyslania)
                .ToList();

            // Oznaczenie przeczytanych wiadomoœci
            foreach (var w in wiadomosci)
            {
                if (w.OdbiorcaId == mojeId && !w.Przeczytana)
                {
                    w.Przeczytana = true;
                }
            }
            _db.SaveChanges();

            ViewBag.Ogloszenie = ogloszenie;
            ViewBag.Inny = inny;
            ViewBag.MojeId = mojeId;

            return View(wiadomosci);
        }

        // Wys³anie nowej wiadomoœci 
        [HttpPost]
        public IActionResult Wyslij(int ogloszenieId, int odbiorcaId, string tresc)
        {
            int mojeId = PobierzIdZalogowanego();

            if (string.IsNullOrWhiteSpace(tresc))
            {
                TempData["Blad"] = "Wiadomoœæ nie mo¿e byæ pusta";
                return RedirectToAction("Konwersacja", new { ogloszenieId, innyId = odbiorcaId });
            }
            if (tresc.Length > 1000)
            {
                TempData["Blad"] = "Wiadomoœæ za d³uga (max 1000 znaków)";
                return RedirectToAction("Konwersacja", new { ogloszenieId, innyId = odbiorcaId });
            }
            if (odbiorcaId == mojeId)
            {
                return RedirectToAction("Szczegoly", "Ogloszenia", new { id = ogloszenieId });
            }

            var ogloszenie = _db.Ogloszenia.Find(ogloszenieId);
            var odbiorca = _db.Uzytkownicy.Find(odbiorcaId);
            if (ogloszenie == null || odbiorca == null) return NotFound();

            var wiadomosc = new Wiadomosc
            {
                Tresc = tresc.Trim(),
                NadawcaId = mojeId,
                OdbiorcaId = odbiorcaId,
                OgloszenieId = ogloszenieId,
                DataWyslania = DateTime.Now,
                Przeczytana = false
            };

            _db.Wiadomosci.Add(wiadomosc);
            _db.SaveChanges();

            return RedirectToAction("Konwersacja", new { ogloszenieId, innyId = odbiorcaId });
        }

        private int PobierzIdZalogowanego()
        {
            var idText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idText!);
        }
    }

    // Klasa pomocnicza do wyœwietlania listy konwersacji w widoku
    public class KonwersacjaPodglad
    {
        public int OgloszenieId { get; set; }
        public int InnyId { get; set; }
        public Ogloszenie Ogloszenie { get; set; } = null!;
        public Uzytkownik Inny { get; set; } = null!;
        public Wiadomosc OstatniaWiadomosc { get; set; } = null!;
        public int Nieprzeczytane { get; set; }
    }
}
