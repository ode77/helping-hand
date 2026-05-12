using HelpingHand.Data;
using HelpingHand.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;
        public RatingRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(VolunteerRating rating)
            => await _context.VolunteerRatings.AddAsync(rating);

        public async Task<IEnumerable<VolunteerRating>>
            GetByVolunteerIdAsync(string volunteerId)
            => await _context.VolunteerRatings
                .Where(r => r.VolunteerId == volunteerId)
                .OrderByDescending(r => r.RatedAt)
                .ToListAsync();

        public async Task<bool> ExistsAsync(
            int requestId, string requesterId)
            => await _context.VolunteerRatings
                .AnyAsync(r => r.HelpRequestId == requestId
                            && r.RequesterId == requesterId);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}