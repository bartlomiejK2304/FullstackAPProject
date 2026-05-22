using System.ComponentModel.DataAnnotations;

namespace FullstackAPPProject.Models;

public class Ogloszenie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Podaj tytuł")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Tytuł od 5 do 150 znaków")]
    [Display(Name = "Tytuł")]
    public string Tytul { get; set; } = "";

    [Required(ErrorMessage = "Podaj opis")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Opis od 10 do 2000 znaków")]
    [Display(Name = "Opis")]
    public string Opis { get; set; } = "";

    [Required(ErrorMessage = "Podaj cenę")]
    [Range(0, 10000, ErrorMessage = "Cena od 0 do 10000 zł")]
    [Display(Name = "Cena za godzinę (zł)")]
    public decimal Cena { get; set; }

    [Required(ErrorMessage = "Podaj miasto")]
    [StringLength(50)]
    [Display(Name = "Miasto")]
    public string Miasto { get; set; } = "";

    
    [Required]
    [Display(Name = "Forma zajęć")]
    public string Forma { get; set; } = "Online";
   
    public DateTime DataDodania { get; set; } = DateTime.Now;
   
    [Required(ErrorMessage = "Wybierz kategorię")]
    [Display(Name = "Kategoria")]
    public int KategoriaId { get; set; }
    public Kategoria? Kategoria { get; set; }
    public int UzytkownikId { get; set; }
    public Uzytkownik? Uzytkownik { get; set; }
}