using System.ComponentModel.DataAnnotations;

namespace FullstackAPPProject.Models;


public class Uzytkownik
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Podaj nazwę użytkownika")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa od 3 do 50 znaków")]
    [Display(Name = "Nazwa użytkownika")]
    public string Nazwa { get; set; } = "";

    [Required(ErrorMessage = "Podaj hasło")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "Hasło min. 4 znaki")]
    [Display(Name = "Hasło")]
    public string Haslo { get; set; } = "";

    //User
    [Required(ErrorMessage = "Wybierz rolę")]
    [Display(Name = "Rola")]
    public string Rola { get; set; } = "Uczen";

    [Display(Name = "O mnie")]
    public string? Opis { get; set; }

    // Zdjęcie
    [Display(Name = "Zdjęcie profilowe")]
    public string? ZdjecieProfilPath { get; set; }

    public List<Ogloszenie> Ogloszenia { get; set; } = new();
}
