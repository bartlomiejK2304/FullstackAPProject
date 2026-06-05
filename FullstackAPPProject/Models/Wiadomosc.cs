using System.ComponentModel.DataAnnotations;

namespace FullstackAPPProject.Models
{
    public class Wiadomosc
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Wpisz treść wiadomości")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Wiadomość max 1000 znaków")]
        [Display(Name = "Treść")]
        public string Tresc { get; set; } = "";

        public DateTime DataWyslania { get; set; } = DateTime.Now;

        // false = nie odczytana przez odbiorcę. Robi się true gdy odbiorca otworzy konwersację.
        public bool Przeczytana { get; set; } = false;

        // Kto wysłał
        public int NadawcaId { get; set; }
        public Uzytkownik? Nadawca { get; set; }

        // Do kogo
        public int OdbiorcaId { get; set; }
        public Uzytkownik? Odbiorca { get; set; }

        // Której oferty dotyczy konwersacja
        public int OgloszenieId { get; set; }
        public Ogloszenie? Ogloszenie { get; set; }
    }

}
