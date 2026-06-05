using System.ComponentModel.DataAnnotations;

namespace FullstackAPPProject.Models
{
    public class Opinia
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Ocena musi być od 1 do 5")]
        [Display(Name = "Ocena")]
        public int Ocena { get; set; }

        [Required(ErrorMessage = "Napisz krótką opinię")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Opinia od 3 do 500 znaków")]
        [Display(Name = "Treść")]
        public string Tresc { get; set; } = "";

        public DateTime DataDodania { get; set; } = DateTime.Now;

        // Kto wystawia 
        public int AutorId { get; set; }
        public Uzytkownik? Autor { get; set; }

        // Kogo dotyczy 
        public int KorepetytorId { get; set; }
        public Uzytkownik? Korepetytor { get; set; }
    }

}
