using HelpdeskModel.ViewModels.Conversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public interface IConversationService
    {
        Task<ConversationThreadViewModel?> GetThreadForUserAsync(long complainId, long userId, ClaimsPrincipal user);
        Task<bool> AddMessageAsync(ConversationMessageViewModel model, long userId);
    }
}
