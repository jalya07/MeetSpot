using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.Entities;

namespace MeetSpot.Infrastructure.Data;

public class MeetSpotAppDbContext : DbContext
{
    public MeetSpotAppDbContext(DbContextOptions<MeetSpotAppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomEquipment> RoomEquipments { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingParticipant> BookingParticipants { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<RoomImage> RoomImages { get; set; }    
    public DbSet<Review> Reviews { get; set; }
    public DbSet<HeroSlide> HeroSlides { get; set; }
    public DbSet<CompanyRequest> CompanyRequests { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<MentorRequest> MentorRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Balance)
            .HasPrecision(18, 2);

        // Room
        modelBuilder.Entity<Room>()
            .Property(r => r.PricePerHour)
            .HasPrecision(18, 2);

        // Booking
        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transaction
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.BalanceAfter)
            .HasPrecision(18, 2);

        // Subscription
        modelBuilder.Entity<Subscription>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithOne(u => u.Subscription)
            .HasForeignKey<Subscription>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoomImage>()
            .HasOne(ri => ri.Room)
            .WithMany(r => r.Images)
            .HasForeignKey(ri => ri.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}