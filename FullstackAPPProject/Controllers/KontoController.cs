using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using FullstackAPPProject.Data;
using FullstackAPPProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FullstackAPPProject.Controllers;

public class KontoController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public KontoController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet]
    public IActionResult Logowanie()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logowanie(string nazwa, string haslo)
    {
        if (string.IsNullOrEmpty(nazwa) || string.IsNullOrEmpty(haslo))
        {
            ViewBag.Blad = "Podaj nazwę użytkownika i hasło";
            return View();
        }

        var hash = DaneStartowe.ZahashujHaslo(haslo);
        var uzytkownik = _db.Uzytkownicy
            .FirstOrDefault(u => u.Nazwa == nazwa && u.Haslo == hash);

        if (uzytkownik == null)
        {
            ViewBag.Blad = "Zła nazwa użytkownika lub hasło";
            return View();
        }

        await Zaloguj(uzytkownik);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Rejestracja()
    {
        return View(new Uzytkownik());
    }

    [HttpPost]
    public async Task<IActionResult> Rejestracja(Uzytkownik model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (_db.Uzytkownicy.Any(u => u.Nazwa == model.Nazwa))
        {
            ViewBag.Blad = "Ta nazwa użytkownika jest już zajęta";
            return View(model);
        }

        var rola = model.Rola == "Korepetytor" ? "Korepetytor" : "Uczen";
        var nowy = new Uzytkownik
        {
            Nazwa = model.Nazwa,
            Haslo = DaneStartowe.ZahashujHaslo(model.Haslo),
            Rola = rola
        };
        _db.Uzytkownicy.Add(nowy);
        _db.SaveChanges();

        await Zaloguj(nowy);
        return RedirectToAction("Panel");
    }

    [Authorize]
    public IActionResult Panel()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }

        int idUzytkownika = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var uzytkownik = _db.Uzytkownicy.Find(idUzytkownika);
        if (uzytkownik == null) return RedirectToAction("Logowanie");

        ViewBag.NieprzeczytaneWiadomosci = _db.Wiadomosci
            .Count(w => w.OdbiorcaId == idUzytkownika && !w.Przeczytana);

        if (uzytkownik.Rola == "Korepetytor")
        {
            var moje = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Where(o => o.UzytkownikId == idUzytkownika)
                .OrderByDescending(o => o.DataDodania)
                .ToList();

            var opinie = _db.Opinie.Where(o => o.KorepetytorId == idUzytkownika).ToList();
            ViewBag.SredniaOcena = opinie.Any() ? opinie.Average(o => o.Ocena) : 0;
            ViewBag.LiczbaOpinii = opinie.Count;

            ViewBag.Uzytkownik = uzytkownik;
            return View("PanelKorepetytora", moje);
        }
        else
        {
            var polecane = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Include(o => o.Uzytkownik)
                .OrderByDescending(o => o.DataDodania)
                .Take(6)
                .ToList();

            ViewBag.Uzytkownik = uzytkownik;
            ViewBag.Kategorie = _db.Kategorie.ToList();
            return View("PanelUcznia", polecane);
        }
    }

    public IActionResult Profil(int id)
    {
        var uzytkownik = _db.Uzytkownicy
            .Include(u => u.Ogloszenia)
                .ThenInclude(o => o.Kategoria)
            .FirstOrDefault(u => u.Id == id);

        if (uzytkownik == null) return NotFound();

        var opinie = _db.Opinie
            .Include(o => o.Autor)
            .Where(o => o.KorepetytorId == id)
            .OrderByDescending(o => o.DataDodania)
            .ToList();

        ViewBag.Opinie = opinie;
        ViewBag.SredniaOcena = opinie.Any() ? opinie.Average(o => o.Ocena) : 0;
        ViewBag.LiczbaOpinii = opinie.Count;

        bool juzOcenil = false;
        if (User.Identity!.IsAuthenticated)
        {
            int mojeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            juzOcenil = opinie.Any(o => o.AutorId == mojeId);
        }
        ViewBag.JuzOcenil = juzOcenil;

        return View(uzytkownik);
    }

    [Authorize]
    [HttpGet]
    public IActionResult EdytujProfil()
    {
        int idUzytkownika = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var uzytkownik = _db.Uzytkownicy.Find(idUzytkownika);
        if (uzytkownik == null) return RedirectToAction("Logowanie");

        return View(uzytkownik);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EdytujProfil(string? opis, IFormFile? zdjecieProfil)
    {
        int idUzytkownika = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var uzytkownik = _db.Uzytkownicy.Find(idUzytkownika);
        if (uzytkownik == null) return RedirectToAction("Logowanie");

        if (opis != null && opis.Length > 500)
        {
            ViewBag.Blad = "Opis może mieć maksymalnie 500 znaków";
            return View(uzytkownik);
        }

        uzytkownik.Opis = opis;

        if (zdjecieProfil != null && zdjecieProfil.Length > 0)
        {
            if (!string.IsNullOrEmpty(uzytkownik.ZdjecieProfilPath))
            {
                var stareFizyczne = Path.Combine(_env.WebRootPath, uzytkownik.ZdjecieProfilPath.TrimStart('/'));
                if (System.IO.File.Exists(stareFizyczne))
                {
                    System.IO.File.Delete(stareFizyczne);
                }
            }

            var folder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var rozszerzenie = Path.GetExtension(zdjecieProfil.FileName);
            var nazwaPliku = "profil_" + Guid.NewGuid() + rozszerzenie;
            var pelnaSciezka = Path.Combine(folder, nazwaPliku);

            using (var stream = new FileStream(pelnaSciezka, FileMode.Create))
            {
                await zdjecieProfil.CopyToAsync(stream);
            }

            uzytkownik.ZdjecieProfilPath = "/uploads/" + nazwaPliku;
        }

        _db.SaveChanges();
        return RedirectToAction("Profil", new { id = uzytkownik.Id });
    }


    [Authorize]
    [HttpGet]
    public IActionResult UsunKonto()
    {
        if (User.IsInRole("Admin"))
        {
            TempData["Blad"] = "Administrator nie może usunąć swojego konta.";
            return RedirectToAction("Panel");
        }

        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UsunKonto(string powod, string potwierdzenie)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Panel");
        }

        int idUzytkownika = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var uzytkownik = _db.Uzytkownicy.Find(idUzytkownika);
        if (uzytkownik == null) return RedirectToAction("Logowanie");

        var wymaganyTekst = "USUN " + uzytkownik.Nazwa;
        if (string.IsNullOrWhiteSpace(potwierdzenie) || potwierdzenie.Trim() != wymaganyTekst)
        {
            ViewBag.Blad = "Niepoprawny tekst potwierdzający. Wpisz dokładnie: " + wymaganyTekst;
            return View();
        }

        if (string.IsNullOrWhiteSpace(powod) || powod.Trim().Length < 5)
        {
            ViewBag.Blad = "Podaj powód usunięcia konta (min. 5 znaków)";
            return View();
        }

        if (powod.Length > 500)
        {
            ViewBag.Blad = "Powód może mieć max 500 znaków";
            return View();
        }

        var log = new UsuniecieKonta
        {
            NazwaUzytkownika = uzytkownik.Nazwa,
            Rola = uzytkownik.Rola,
            Powod = powod.Trim(),
            DataUsuniecia = DateTime.Now
        };
        _db.UsunieciaKont.Add(log);

        var wiadomosci = _db.Wiadomosci.Where(w => w.NadawcaId == idUzytkownika || w.OdbiorcaId == idUzytkownika);
        _db.Wiadomosci.RemoveRange(wiadomosci);

        var opinie = _db.Opinie.Where(o => o.AutorId == idUzytkownika || o.KorepetytorId == idUzytkownika);
        _db.Opinie.RemoveRange(opinie);

        var ogloszenia = _db.Ogloszenia.Where(o => o.UzytkownikId == idUzytkownika);
        _db.Ogloszenia.RemoveRange(ogloszenia);

        _db.Uzytkownicy.Remove(uzytkownik);
        _db.SaveChanges();

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["KontoUsuniete"] = "Twoje konto zostało usunięte. Dziękujemy za korzystanie z KorkiPL.";
        return RedirectToAction("Logowanie");
    }

    public async Task<IActionResult> Wyloguj()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task Zaloguj(Uzytkownik u)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new Claim(ClaimTypes.Name, u.Nazwa),
            new Claim(ClaimTypes.Role, u.Rola)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
