using FullstackAPPProject.Data;
using FullstackAPPProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FullstackAPPProject.Controllers
{
    public class OgloszeniaController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public OgloszeniaController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }


        public IActionResult Index(string? szukaj, int? kategoriaId, decimal? cenaMin, decimal? cenaMax, string? forma)
        {
            var lista = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Include(o => o.Uzytkownik)
                .AsQueryable();

            if (!string.IsNullOrEmpty(szukaj))
            {
                lista = lista.Where(o => o.Tytul.Contains(szukaj));
            }

            if (kategoriaId.HasValue)
            {
                lista = lista.Where(o => o.KategoriaId == kategoriaId.Value);
            }


            if (cenaMin.HasValue)
            {
                lista = lista.Where(o => o.Cena >= cenaMin.Value);
            }
            if (cenaMax.HasValue)
            {
                lista = lista.Where(o => o.Cena <= cenaMax.Value);
            }


            if (!string.IsNullOrEmpty(forma) && forma != "Wszystkie")
            {
                lista = lista.Where(o => o.Forma == forma);
            }

            var wynik = lista.OrderByDescending(o => o.DataDodania).ToList();

            ViewBag.Kategorie = _db.Kategorie.ToList();
            ViewBag.Szukaj = szukaj;
            ViewBag.WybranaKategoria = kategoriaId;
            ViewBag.CenaMin = cenaMin;
            ViewBag.CenaMax = cenaMax;
            ViewBag.Forma = forma;

            return View(wynik);
        }


        public IActionResult Szczegoly(int id)
        {
            var ogloszenie = _db.Ogloszenia
                .Include(o => o.Kategoria)
                .Include(o => o.Uzytkownik)
                .FirstOrDefault(o => o.Id == id);

            if (ogloszenie == null)
                return NotFound();

           
            var opinie = _db.Opinie.Where(o => o.KorepetytorId == ogloszenie.UzytkownikId).ToList();
            ViewBag.SredniaOcena = opinie.Any() ? opinie.Average(o => o.Ocena) : 0;
            ViewBag.LiczbaOpinii = opinie.Count;

            return View(ogloszenie);
        }


        [Authorize]
        [HttpGet]
        public IActionResult Dodaj()
        {
            
            if (!CzyKorepetytor())
                return RedirectToAction("Panel", "Konto");

            WstawKategorieDoViewBag();
            return View(new Ogloszenie());
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Dodaj(Ogloszenie model, IFormFile? zdjecie)
        {
            if (!CzyKorepetytor())
                return RedirectToAction("Panel", "Konto");

            ModelState.Remove("Uzytkownik");
            ModelState.Remove("Kategoria");
            ModelState.Remove("ZdjeciePath");

            if (!ModelState.IsValid)
            {
                WstawKategorieDoViewBag();
                return View(model);
            }

            
            if (zdjecie != null && zdjecie.Length > 0)
            {
                model.ZdjeciePath = await ZapiszZdjecie(zdjecie);
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
        public async Task<IActionResult> Edytuj(Ogloszenie model, IFormFile? zdjecie)
        {
            ModelState.Remove("Uzytkownik");
            ModelState.Remove("Kategoria");
            ModelState.Remove("ZdjeciePath");

            if (!ModelState.IsValid)
            {
                WstawKategorieDoViewBag();
                return View(model);
            }

            var ogloszenie = _db.Ogloszenia.Find(model.Id);
            if (ogloszenie == null)
                return NotFound();

            if (ogloszenie.UzytkownikId != PobierzIdZalogowanego())
                return Forbid();

            ogloszenie.Tytul = model.Tytul;
            ogloszenie.Opis = model.Opis;
            ogloszenie.Cena = model.Cena;
            ogloszenie.Miasto = model.Miasto;
            ogloszenie.Forma = model.Forma;
            ogloszenie.KategoriaId = model.KategoriaId;

           
            if (zdjecie != null && zdjecie.Length > 0)
            {
                UsunZdjecieZDysku(ogloszenie.ZdjeciePath);
                ogloszenie.ZdjeciePath = await ZapiszZdjecie(zdjecie);
            }

            _db.SaveChanges();
            return RedirectToAction("Szczegoly", new { id = ogloszenie.Id });
        }


        [Authorize]
        [HttpPost]
        public IActionResult Usun(int id)
        {
            var ogloszenie = _db.Ogloszenia.Find(id);
            if (ogloszenie == null)
                return NotFound();

            if (ogloszenie.UzytkownikId != PobierzIdZalogowanego())
                return Forbid();

            UsunZdjecieZDysku(ogloszenie.ZdjeciePath);
            _db.Ogloszenia.Remove(ogloszenie);
            _db.SaveChanges();
            return RedirectToAction("Panel", "Konto");
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

        private bool CzyKorepetytor()
        {
            return User.IsInRole("Korepetytor");
        }


        private void WstawKategorieDoViewBag()
        {
            var kategorie = _db.Kategorie.OrderBy(k => k.Nazwa).ToList();
            ViewBag.Kategorie = new SelectList(kategorie, "Id", "Nazwa");
        }

      
        private async Task<string> ZapiszZdjecie(IFormFile zdjecie)
        {
            
            var folder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

           
            var rozszerzenie = Path.GetExtension(zdjecie.FileName);
            var nazwaPliku = Guid.NewGuid().ToString() + rozszerzenie;
            var pelnaSciezka = Path.Combine(folder, nazwaPliku);

            using (var stream = new FileStream(pelnaSciezka, FileMode.Create))
            {
                await zdjecie.CopyToAsync(stream);
            }

            return "/uploads/" + nazwaPliku;
        }

        
        private void UsunZdjecieZDysku(string? sciezka)
        {
            if (string.IsNullOrEmpty(sciezka)) return;
            
            var fizyczna = Path.Combine(_env.WebRootPath, sciezka.TrimStart('/'));
            if (System.IO.File.Exists(fizyczna))
            {
                System.IO.File.Delete(fizyczna);
            }
        }
    }

}