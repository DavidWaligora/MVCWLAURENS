using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StartspelerAPI.Models;

namespace StartspelerAPI.Data
{
    public class StartspelerAPIContext : IdentityDbContext
    {
        public StartspelerAPIContext(DbContextOptions<StartspelerAPIContext> options) : base(options)
        {
            //niets binnendoen
        }

        public DbSet<Community> Communities { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // tabellen aanmaken
            modelBuilder.Entity<Community>().ToTable("Communities");
            modelBuilder.Entity<Event>().ToTable("Events");
            modelBuilder.Entity<Inschrijving>().ToTable("Inschrijvingen");

            // One to many
            modelBuilder.Entity<Inschrijving>()
                .HasOne(i => i.Event)
                .WithMany(e => e.Inschrijvingen)
                .HasForeignKey(y => y.EventId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Inschrijving>()
                .HasOne(i => i.Gebruiker)
                .WithMany(g => g.Inschrijvingen)
                .HasForeignKey(y => y.GebruikerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Community)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CommunityId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }

    }
}
