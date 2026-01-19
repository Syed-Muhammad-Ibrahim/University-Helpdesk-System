using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.IRepository
{
    public interface INotificationRepository
    {
        Task AddRangeAsync(List<Notification> items);
        Task<List<Notification>> GetUnreadByUserIdAsync(long userId);
        Task MarkAsReadAsync(long notificationId, long userId);
        Task SaveChangesAsync();
        Task<Notification?> GetByUserAndComplainAsync(long userId, long complainId);
        Task AddAsync(Notification notification);
        Task<Notification?> GetByIdForUserAsync(long id, long userId);
    }

}
