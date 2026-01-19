using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public interface INotificationService
    {
        Task CreateReplyNotificationAsync(long complainId, long senderUserId);
        Task<List<Notification>> GetUnreadAsync(long userId);
        Task MarkAsReadAsync(long notificationId, long userId);
    }
}
