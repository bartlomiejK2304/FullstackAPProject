using System.Security.Claims;
using Projekt.Data;
using Projekt.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FullstackAPPProject.Controllers;

public class KontoController : Controller
{
    private readonly AppDbContext _db;

    public KontoController(AppDbContext db)
    {
        _db = db;
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

        var nowy = new Uzytkownik
        {
            Nazwa = model.Nazwa,
            Haslo = DaneStartowe.ZahashujHaslo(model.Haslo)
        };
        _db.Uzytkownicy.Add(nowy);
        _db.SaveChanges();

        await Zaloguj(nowy);
        return RedirectToAction("Index", "Home");
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
            new Claim(ClaimTypes.Name, u.Nazwa)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
