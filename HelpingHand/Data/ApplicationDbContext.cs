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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<HelpRequest>()
                .HasIndex(h => new { h.Status, h.CreatedAt });

            builder.Entity<HelpRequest>()
                .HasOne(h => h.Requester)
                .WithMany(u => u.PostedRequests)
                .HasForeignKey(h => h.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HelpRequest>()
                .HasOne(h => h.Volunteer)
                .WithMany(u => u.ClaimedRequests)
                .HasForeignKey(h => h.VolunteerId)
                .OnDelete(DeleteBehavior.SetNull);

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