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
    public class NotificationRepository : INotificationRepository
    {
            private readonly AppDbContext _context;
            public NotificationRepository(AppDbContext context) { this._context = context; }

            public async Task AddRangeAsync(List<Notification> items)
                => await _context.Notifications.AddRangeAsync(items);

        public async Task<List<Notification>> GetUnreadByUserIdAsync(long userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && n.UnreadCount > 0)
                .OrderByDescending(n => n.LastAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdForUserAsync(long id, long userId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        }

        public async Task MarkAsReadAsync(long notificationId, long userId)
        {
            var n = await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);

            if (n == null) return;

            n.IsRead = true;
            n.UnreadCount = 0;
        }

        public async Task<Notification?> GetByUserAndComplainAsync(long userId, long complainId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.UserId == userId && n.ComplainId == complainId);
        }

        public async Task AddAsync(Notification notification)
            => await _context.Notifications.AddAsync(notification);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

    }

}
