using HelpingHand.Data;
using HelpingHand.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Repositories
{
    public class VolunteerApplicationRepository : IVolunteerApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public VolunteerApplicationRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(VolunteerApplication application)
            => await _context.VolunteerApplications.AddAsync(application);

        public async Task<IEnumerable<VolunteerApplication>> GetByRequestIdAsync(int requestId)
            => await _context.VolunteerApplications
                .Include(a => a.Volunteer)
                .Where(a => a.HelpRequestId == requestId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

        public async Task<VolunteerApplication?> GetByIdAsync(int id)
            => await _context.VolunteerApplications
                .Include(a => a.Volunteer)
                .FirstOrDefaultAsync(a => a.VolunteerApplicationId == id);

        // Lets the controller check for duplicate applications
        public async Task<VolunteerApplication?> GetByRequestAndVolunteerAsync(
            int requestId, string volunteerId)
            => await _context.VolunteerApplications
                .FirstOrDefaultAsync(a =>
                    a.HelpRequestId == requestId &&
                    a.VolunteerId == volunteerId);

        public Task UpdateAsync(VolunteerApplication application)
        {
            _context.VolunteerApplications.Update(application);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}