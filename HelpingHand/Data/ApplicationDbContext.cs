using HelpingHand.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<HelpRequest> HelpRequests { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<VolunteerApplication> VolunteerApplications { get; set; }
        public DbSet<RequestComment> RequestComments { get; set; }
        public DbSet<VolunteerRating> VolunteerRatings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RequestTemplate> RequestTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Composite index for help board performance
            builder.Entity<HelpRequest>()
                .HasIndex(h => new { h.Status, h.CreatedAt });

            // Urgency index for sorting
            builder.Entity<HelpRequest>()
                .HasIndex(h => h.Urgency);

            // Requester FK
            builder.Entity<HelpRequest>()
                .HasOne(h => h.Requester)
                .WithMany(u => u.PostedRequests)
                .HasForeignKey(h => h.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Volunteer FK
            builder.Entity<HelpRequest>()
                .HasOne(h => h.Volunteer)
                .WithMany(u => u.ClaimedRequests)
                .HasForeignKey(h => h.VolunteerId)
                .OnDelete(DeleteBehavior.SetNull);

            // VolunteerApplication FK
            builder.Entity<VolunteerApplication>()
                .HasOne(a => a.Volunteer)
                .WithMany()
                .HasForeignKey(a => a.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            // RequestComment FK
            builder.Entity<RequestComment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // VolunteerRating FK
            builder.Entity<VolunteerRating>()
                .HasOne(r => r.Volunteer)
                .WithMany(u => u.RatingsReceived)
                .HasForeignKey(r => r.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification FK
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed categories
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Grocery Collection" },
                new Category { CategoryId = 2, Name = "Transport" },
                new Category { CategoryId = 3, Name = "Technical Support" },
                new Category { CategoryId = 4, Name = "Manual Labour" },
                new Category { CategoryId = 5, Name = "General Volunteering" }
            );
        }
    }
}