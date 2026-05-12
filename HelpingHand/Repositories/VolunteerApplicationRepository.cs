using HelpingHand.Data;
using HelpingHand.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Repositories
{
    public class VolunteerApplicationRepository
        : IVolunteerApplicationRepository
    {
        private readonly ApplicationDbContext _context;
        public VolunteerApplicationRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(VolunteerApplication application)
            => await _context.VolunteerApplications.AddAsync(application);

        public async Task<IEnumerable<VolunteerApplication>>
            GetByRequestIdAsync(int requestId)
            => await _context.VolunteerApplications
                .Include(a => a.Volunteer)
                .Where(a => a.HelpRequestId == requestId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

        public async Task<VolunteerApplication?> GetByIdAsync(int id)
            => await _context.VolunteerApplications
                .Include(a => a.Volunteer)
                .FirstOrDefaultAsync(a => a.VolunteerApplicationId == id);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}