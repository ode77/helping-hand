using HelpingHand.Data;
using HelpingHand.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(RequestComment comment)
            => await _context.RequestComments.AddAsync(comment);

        public async Task<IEnumerable<RequestComment>>
            GetByRequestIdAsync(int requestId)
            => await _context.RequestComments
                .Include(c => c.Author)
                .Where(c => c.HelpRequestId == requestId)
                .OrderBy(c => c.PostedAt)
                .ToListAsync();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}