using HelpingHand.Data;
using HelpingHand.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly ApplicationDbContext _context;
        public TemplateRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(RequestTemplate template)
            => await _context.RequestTemplates.AddAsync(template);

        public async Task<IEnumerable<RequestTemplate>>
            GetByOwnerIdAsync(string ownerId)
            => await _context.RequestTemplates
                .Include(t => t.Category)
                .Where(t => t.OwnerId == ownerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<RequestTemplate?> GetByIdAsync(int id)
            => await _context.RequestTemplates
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.RequestTemplateId == id);

        public async Task DeleteAsync(int id)
        {
            var t = await _context.RequestTemplates.FindAsync(id);
            if (t != null) _context.RequestTemplates.Remove(t);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}