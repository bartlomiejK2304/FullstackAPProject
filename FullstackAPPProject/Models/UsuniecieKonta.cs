using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace FullstackAPPProject.Models;
public class UsuniecieKonta
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string NazwaUzytkownika { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string Rola { get; set; } = "";

    [Required(ErrorMessage = "Podaj powód usunięcia konta")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Powód od 5 do 500 znaków")]
    [Display(Name = "Powód usunięcia")]
    public string Powod { get; set; } = "";

    public DateTime DataUsuniecia { get; set; } = DateTime.Now;
}


