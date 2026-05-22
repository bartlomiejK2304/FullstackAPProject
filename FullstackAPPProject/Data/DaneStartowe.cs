using FullstackAPPProject.Models;

namespace FullstackAPPProject.Data
{
    public static class DaneStartowe
    {
        public static void Wypelnij(AppDbContext db)
        {
            // Jeśli są już kategorie, nic nie rób
            if (db.Kategorie.Any()) return;

            // Dodaj kategorie
            var matematyka = new Kategoria { Nazwa = "Matematyka" };
            var fizyka = new Kategoria { Nazwa = "Fizyka" };
            var angielski = new Kategoria { Nazwa = "Język angielski" };
            var polski = new Kategoria { Nazwa = "Język polski" };
            var informatyka = new Kategoria { Nazwa = "Informatyka" };
            var chemia = new Kategoria { Nazwa = "Chemia" };

            db.Kategorie.AddRange(matematyka, fizyka, angielski, polski, informatyka, chemia);
            db.SaveChanges();

            // Dodaj przykładowych użytkowników (hasło: "haslo")
            var anna = new Uzytkownik { Nazwa = "anna", Haslo = ZahashujHaslo("haslo") };
            var piotr = new Uzytkownik { Nazwa = "piotr", Haslo = ZahashujHaslo("haslo") };
            db.Uzytkownicy.AddRange(anna, piotr);
            db.SaveChanges();

            // Dodaj przykładowe ogłoszenia
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
                }
            );
            db.SaveChanges();
        }

        // Prosty hash hasła (SHA256)
        public static string ZahashujHaslo(string haslo)
        {
            var bajty = System.Text.Encoding.UTF8.GetBytes(haslo);
            var hash = System.Security.Cryptography.SHA256.HashData(bajty);
            return Convert.ToBase64String(hash);
        }

    }
}
