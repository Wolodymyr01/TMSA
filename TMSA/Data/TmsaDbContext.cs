using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data
{
    public class TmsaDbContext : DbContext, ITmsaDbContext
    {
        public TmsaDbContext(DbContextOptions<TmsaDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Event> Events { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Client)
                .WithMany(c => c.GetBookings);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany();
            modelBuilder.Entity<Client>()
                .Property(c => c.FirstName)
                .IsRequired()
                .HasColumnType("nvarchar(50)");
            modelBuilder.Entity<Client>()
                .Property(c => c.LastName)
                .IsRequired()
                .HasColumnType("nvarchar(75)");
            modelBuilder.Entity<Client>()
                .Property(c => c.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");
            modelBuilder.Entity<Event>()
                .Property(e => e.Name)
                .IsRequired()
                .HasColumnType("nvarchar(75)");
            modelBuilder.Entity<Event>()
                .Property(e => e.Description)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
        }
    }
}