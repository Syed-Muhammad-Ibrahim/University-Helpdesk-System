using HelpdeskModel.Models;
using HelpdeskRepository.Data;
using HelpdeskRepository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.Repository
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _context;

        public ConversationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetByComplainIdAsync(long complainId)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.ComplainId == complainId);
        }

        public async Task<Conversation?> GetByIdAsync(long id)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<ConversationLog>> GetLogsByConversationIdAsync(long conversationId)
        {
            return await _context.ConversationLogs
                .Include(l => l.CreatedBy)
                .Where(l => l.ConversationId == conversationId)
                .OrderBy(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task AddConversationAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
        }

        public async Task AddLogAsync(ConversationLog log)
        {
            await _context.ConversationLogs.AddAsync(log);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
