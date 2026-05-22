using System.ComponentModel.DataAnnotations;

namespace FullstackAPPProject.Models;


public class Kategoria
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Nazwa { get; set; } = "";

    public List<Ogloszenie> Ogloszenia { get; set; } = new();
}
