# KorkiPL — Platforma korepetycji online

Webowa aplikacja umo¿liwiaj¹ca korepetytorum publikowanie og³oszeñ z ofertami zajêæ, a uczniom przegl¹danie ofert, kontaktowanie siê z korepetytorami oraz wystawianie opinii. Aplikacja posiada panel administracyjny do zarz¹dzania treœciami serwisu.


## U¿yte technologie i ich zastosowania

- **ASP.NET Core MVC, wersja 8.0** - Framework webowy, serwer HTTP, routing
- **C# 12** - Jêzyk programowania backendu
- **Entity Framework Core, wersja 8.0.27** - ORM — mapowanie obiektowo-relacyjne
- **SQLite, wersja 3.x** -  Baza danych (plik "korki.db")
- **Razor Views** - Silnik szablonów HTML po stronie serwera
- **HTML5 + CSS3** - Struktura i stylowanie interfejsu
- **JavaScript (Vanilla), wersja ES6+** - Interaktywnoœæ (hamburger menu, dropdown, podgl¹d zdjêæ)
- **Bootstrap Icons, wersja 1.11.3** - Ikony w interfejsie
- **Cookie Authentication** - Uwierzytelnianie i autoryzacja u¿ytkowników

## G³ówne funkcjonalnoœci

- **System og³oszeñ** — tworzenie, edycja, usuwanie, przegl¹danie z filtrowaniem po kategorii i mieœcie
- **System wiadomoœci** — konwersacje miêdzy uczniami a korepetytorami powi¹zane z og³oszeniami
- **System opinii** — oceny 1–5 gwiazdek z komentarzem dla korepetytorów
- **Panel administracyjny** — dashboard ze statystykami, zarz¹dzanie u¿ytkownikami, og³oszeniami, kategoriami
- **Trzy role u¿ytkowników** — Administrator, Korepetytor, Uczeñ (ró¿na nawigacja i uprawnienia)
- **Zarz¹dzanie kontem** — rejestracja, logowanie, edycja profilu, usuwanie konta z potwierdzeniem
- **Responsywny interfejs** — hamburger menu na mobile, user dropdown w nawigacji

## Instrukcja uruchomienia

### Wymagania

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) lub nowszy
- Dowolny edytor (Visual Studio 2022, VS Code, Rider)

### Kroki

1. **Sklonuj repozytorium:**
   
   git clone <URL_REPOZYTORIUM>

   cd projektzaliczeniwoy
   

2. **PrzejdŸ do folderu projektu:**
   
   cd FullstackAPPProject
   

3. **Przywróæ pakiety NuGet:**
   
   dotnet restore
   

4. **Uruchom aplikacjê:**
   
   dotnet run
   

5. **Otwórz przegl¹darkê** pod adresem wyœwietlonym w terminalu (domyœlnie `http://localhost:5163`).

6.  Baza danych `korki.db` zostanie utworzona automatycznie z przyk³adowymi danymi.

### Konta testowe

| Login | Has³o | Rola |
|---|---|---|
| `admin` | `admin` | Administrator |
| `anna` | `haslo` | Korepetytor |
| `piotr` | `haslo` | Korepetytor |
| `kasia` | `haslo` | Uczeñ |

### Uwagi

- Baza SQLite tworzy siê automatycznie — nie wymaga zewnêtrznego serwera
- Aby zresetowaæ dane: usuñ plik "korki.db" i uruchom ponownie
- Zdjêcia u¿ytkowników zapisywane s¹ w `wwwroot/uploads/