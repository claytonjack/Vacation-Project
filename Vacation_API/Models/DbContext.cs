using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VacationBooking.Models;

namespace VacationBooking.Data
{
    public class VacationDbContext : IdentityDbContext<User>
    {
        public VacationDbContext(DbContextOptions<VacationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vacation)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VacationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.Destination)
                .WithMany(d => d.Vacations)
                .HasForeignKey(v => v.DestinationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.Accommodation)
                .WithMany(a => a.Vacations)
                .HasForeignKey(v => v.AccommodationID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}