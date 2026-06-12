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
        public DbSet<Wiadomosc> Wiadomosci { get; set; }
        public DbSet<Opinia> Opinie { get; set; }
        public DbSet<UsuniecieKonta> UsunieciaKont { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wiadomosc>()
                .HasOne(w => w.Nadawca) 
                .WithMany() 
                .HasForeignKey(w => w.NadawcaId) 
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Wiadomosc>()
                .HasOne(w => w.Odbiorca)
                .WithMany()
                .HasForeignKey(w => w.OdbiorcaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opinia>()
                .HasOne(o => o.Autor)
                .WithMany()
                .HasForeignKey(o => o.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opinia>()
                .HasOne(o => o.Korepetytor)
                .WithMany()
                .HasForeignKey(o => o.KorepetytorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}
