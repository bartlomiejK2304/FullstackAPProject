using FullstackAPPProject.Models;

namespace FullstackAPPProject.Data
{
    public static class DaneStartowe
    {
        public static void Wypelnij(AppDbContext db)
        {
            if (db.Kategorie.Any()) return;

            var matematyka = new Kategoria { Nazwa = "Matematyka" };
            var fizyka = new Kategoria { Nazwa = "Fizyka" };
            var angielski = new Kategoria { Nazwa = "Język angielski" };
            var polski = new Kategoria { Nazwa = "Język polski" };
            var informatyka = new Kategoria { Nazwa = "Informatyka" };
            var chemia = new Kategoria { Nazwa = "Chemia" };

            db.Kategorie.AddRange(matematyka, fizyka, angielski, polski, informatyka, chemia);
            db.SaveChanges();
          
            var admin = new Uzytkownik { Nazwa = "admin", Haslo = ZahashujHaslo("admin"), Rola = "Admin" };
            var anna = new Uzytkownik { Nazwa = "anna", Haslo = ZahashujHaslo("haslo") };
            var piotr = new Uzytkownik { Nazwa = "piotr", Haslo = ZahashujHaslo("haslo") };
            var kasia = new Uzytkownik { Nazwa = "kasia", Haslo = ZahashujHaslo("haslo"), Rola = "Uczen"};

            db.Uzytkownicy.AddRange(anna, piotr, kasia);
            db.SaveChanges();

            db.Ogloszenia.AddRange(
                new Ogloszenie
                {
                    Tytul = "Korepetycje z matematyki - liceum",
                    Opis = "Pomogę Ci zrozumieć matematykę. Wieloletnie doświadczenie z uczniami liceum.",
                    Cena = 80,
                    Miasto = "Warszawa",
                    Forma = "Online",
                    KategoriaId = matematyka.Id,
                    UzytkownikId = anna.Id,
                    DataDodania = DateTime.Now.AddDays(-3)
                },
                new Ogloszenie
                {
                    Tytul = "Język angielski - konwersacje",
                    Opis = "Konwersacje na każdym poziomie. Przygotowanie do matury i certyfikatów.",
                    Cena = 100,
                    Miasto = "Kraków",
                    Forma = "Online",
                    KategoriaId = angielski.Id,
                    UzytkownikId = piotr.Id,
                    DataDodania = DateTime.Now.AddDays(-1)
                },
                new Ogloszenie
                {
                    Tytul = "Programowanie w Pythonie od podstaw",
                    Opis = "Nauczę Cię programować w Pythonie. Od zmiennych po obiekty i bazy danych.",
                    Cena = 120,
                    Miasto = "Wrocław",
                    Forma = "Online",
                    KategoriaId = informatyka.Id,
                    UzytkownikId = piotr.Id,
                    DataDodania = DateTime.Now.AddHours(-5)
                },
                new Ogloszenie
                {
                    Tytul = "Fizyka - przygotowanie do matury",
                    Opis = "Magister fizyki, pomoc w przygotowaniach do matury podstawowej i rozszerzonej.",
                    Cena = 90,
                    Miasto = "Warszawa",
                    Forma = "Stacjonarnie",
                    KategoriaId = fizyka.Id,
                    UzytkownikId = anna.Id,
                    DataDodania = DateTime.Now.AddHours(-12)
                },
                new Ogloszenie
                {
                    Tytul = "Chemia organiczna - studia",
                    Opis = "Doktorant chemii organicznej, pomoc dla studentów kierunków przyrodniczych.",
                    Cena = 110,
                    Miasto = "Poznań",
                    Forma = "Online",
                    KategoriaId = chemia.Id,
                    UzytkownikId = anna.Id,
                    DataDodania = DateTime.Now.AddHours(-2)
                },
                new Ogloszenie
                {
                    Tytul = "Język polski - matura ustna i pisemna",
                    Opis = "Polonistka po UJ. Pomogę z lekturami, rozprawkami i interpretacją.",
                    Cena = 85,
                    Miasto = "Kraków",
                    Forma = "Stacjonarnie",
                    KategoriaId = polski.Id,
                    UzytkownikId = piotr.Id,
                    DataDodania = DateTime.Now.AddHours(-20)
                }

            );
            db.SaveChanges();
            
            var ogloszenieMat = db.Ogloszenia.First(o => o.UzytkownikId == anna.Id && o.KategoriaId == matematyka.Id);
            var ogloszeniePython = db.Ogloszenia.First(o => o.UzytkownikId == piotr.Id && o.KategoriaId == informatyka.Id);

            db.Wiadomosci.AddRange(
                
                new Wiadomosc
                {
                    Tresc = "Cześć! Chciałabym się umówić na korki z matmy. Jakie masz wolne terminy?",
                    NadawcaId = kasia.Id,
                    OdbiorcaId = anna.Id,
                    OgloszenieId = ogloszenieMat.Id,
                    DataWyslania = DateTime.Now.AddHours(-2),
                    Przeczytana = true
                },
                new Wiadomosc
                {
                    Tresc = "Cześć Kasia! Mogę w środy i piątki po 16:00. Pasuje?",
                    NadawcaId = anna.Id,
                    OdbiorcaId = kasia.Id,
                    OgloszenieId = ogloszenieMat.Id,
                    DataWyslania = DateTime.Now.AddHours(-1),
                    Przeczytana = false
                },
                
                new Wiadomosc
                {
                    Tresc = "Dzień dobry, czy uczy Pan też zaawansowanego Pythona (Django, REST)?",
                    NadawcaId = kasia.Id,
                    OdbiorcaId = piotr.Id,
                    OgloszenieId = ogloszeniePython.Id,
                    DataWyslania = DateTime.Now.AddMinutes(-30),
                    Przeczytana = false
                }
            );
            db.SaveChanges();

            
            db.Opinie.AddRange(
                new Opinia
                {
                    Ocena = 5,
                    Tresc = "Świetna nauczycielka! Wreszcie zrozumiałam pochodne. Bardzo polecam.",
                    AutorId = kasia.Id,
                    KorepetytorId = anna.Id,
                    DataDodania = DateTime.Now.AddDays(-2)
                },
                new Opinia
                {
                    Ocena = 4,
                    Tresc = "Bardzo dobre lekcje, spokojne tempo, dużo praktyki. Mogłoby być więcej projektów.",
                    AutorId = kasia.Id,
                    KorepetytorId = piotr.Id,
                    DataDodania = DateTime.Now.AddDays(-1)
                }
            );
            db.SaveChanges();

        }

        public static string ZahashujHaslo(string haslo)
        {
            var bajty = System.Text.Encoding.UTF8.GetBytes(haslo);
            var hash = System.Security.Cryptography.SHA256.HashData(bajty);
            return Convert.ToBase64String(hash);
        }

    }
}
