using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.IRepository
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByComplainIdAsync(long complainId);
        Task<Conversation?> GetByIdAsync(long id);
        Task<List<ConversationLog>> GetLogsByConversationIdAsync(long conversationId);
        Task AddConversationAsync(Conversation conversation);
        Task AddLogAsync(ConversationLog log);
        Task SaveChangesAsync();
    }
}
