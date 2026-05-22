using FullstackAPPProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FullstackAPPProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        public DbSet<Kategoria> Kategorie { get; set; }
        public DbSet<Ogloszenie> Ogloszenia { get; set; }
    }

}
