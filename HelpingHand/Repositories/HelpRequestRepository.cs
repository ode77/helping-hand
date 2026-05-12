using HelpingHand.Data;
using HelpingHand.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Repositories
{
    public class HelpRequestRepository : IHelpRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public HelpRequestRepository(ApplicationDbContext context)
            => _context = context;

        // Only Open requests that have not expired
        // Sorted urgent first then most recent
        public async Task<IEnumerable<HelpRequest>>
            GetOpenRequestsAsync()
            => await _context.HelpRequests
                .AsNoTracking()
                .Include(h => h.Category)
                .Include(h => h.Requester)
                .Where(h => h.Status == RequestStatus.Open
                         && h.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(h => h.Urgency)
                .ThenByDescending(h => h.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<HelpRequest>> GetAllAsync()
            => await _context.HelpRequests
                .AsNoTracking()
                .Include(h => h.Category)
                .Include(h => h.Requester)
                .Include(h => h.Volunteer)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

        public async Task<HelpRequest?> GetByIdAsync(int id)
            => await _context.HelpRequests
                .Include(h => h.Category)
                .Include(h => h.Requester)
                .Include(h => h.Volunteer)
                .Include(h => h.Applications)
                    .ThenInclude(a => a.Volunteer)
                .Include(h => h.Comments)
                    .ThenInclude(c => c.Author)
                .Include(h => h.Ratings)
                .FirstOrDefaultAsync(h => h.HelpRequestId == id);

        public async Task<IEnumerable<HelpRequest>>
            GetByRequesterIdAsync(string userId)
            => await _context.HelpRequests
                .AsNoTracking()
                .Include(h => h.Category)
                .Include(h => h.Volunteer)
                .Where(h => h.RequesterId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<HelpRequest>>
            GetByVolunteerIdAsync(string userId)
            => await _context.HelpRequests
                .AsNoTracking()
                .Include(h => h.Category)
                .Include(h => h.Requester)
                .Where(h => h.VolunteerId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(HelpRequest request)
            => await _context.HelpRequests.AddAsync(request);

        public Task UpdateAsync(HelpRequest request)
        {
            _context.HelpRequests.Update(request);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var r = await _context.HelpRequests.FindAsync(id);
            if (r != null) _context.HelpRequests.Remove(r);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}