using FullstackAPPProject.Data;
using FullstackAPPProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FullstackAPPProject.Controllers
{
    [Authorize] 
    public class OpinieController : Controller
    {
        private readonly AppDbContext _db;

        public OpinieController(AppDbContext db)
        {
            _db = db;
        }

        // Dodawanie opinii o korepetytorze
        [HttpPost]
        public IActionResult Dodaj(int korepetytorId, int ocena, string tresc)
        {
            int mojeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (korepetytorId == mojeId)
            {
                TempData["BladOpinia"] = "Nie mo¿esz oceniaæ samego siebie";
                return RedirectToAction("Profil", "Konto", new { id = korepetytorId });
            }

            var korepetytor = _db.Uzytkownicy.Find(korepetytorId);
            if (korepetytor == null) return NotFound();

            if (korepetytor.Rola != "Korepetytor")
            {
                TempData["BladOpinia"] = "Mo¿na oceniaæ tylko korepetytorów";
                return RedirectToAction("Profil", "Konto", new { id = korepetytorId });
            }

            // Poprawnoœc oceniania
            if (ocena < 1 || ocena > 5)
            {
                TempData["BladOpinia"] = "Ocena musi byæ od 1 do 5";
                return RedirectToAction("Profil", "Konto", new { id = korepetytorId });
            }

            if (string.IsNullOrWhiteSpace(tresc) || tresc.Length < 3)
            {
                TempData["BladOpinia"] = "Wpisz krótk¹ opiniê (min. 3 znaki)";
                return RedirectToAction("Profil", "Konto", new { id = korepetytorId });
            }

            if (tresc.Length > 500)
            {
                TempData["BladOpinia"] = "Opinia mo¿e mieæ max 500 znaków";
                return RedirectToAction("Profil", "Konto", new { id = korepetytorId });
            }

            bool juzOcenil = _db.Opinie.Any(o => o.AutorId == mojeId && o.KorepetytorId == korepetytorId);
            if (juzOcenil)
            {
                TempData["BladOpinia"] = "Ju¿ oceni³eœ tego korepetytora";
                return RedirectToAction("Profil", "Konto", new { id = korepetytorId });
            }

            var opinia = new Opinia
            {
                Ocena = ocena,
                Tresc = tresc.Trim(),
                AutorId = mojeId,
                KorepetytorId = korepetytorId,
                DataDodania = DateTime.Now
            };

            _db.Opinie.Add(opinia);
            _db.SaveChanges();

            return RedirectToAction("Profil", "Konto", new { id = korepetytorId });
        }
    }
}
