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
            private readonly AppDbContext context;
            public NotificationRepository(AppDbContext context) { this.context = context; }

            public async Task AddRangeAsync(List<Notification> items)
                => await context.Notifications.AddRangeAsync(items);

            public async Task<List<Notification>> GetUnreadByUserIdAsync(long userId)
                => await context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

            public async Task MarkAsReadAsync(long notificationId, long userId)
            {
                var n = await context.Notifications
                    .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);

                if (n == null) return;
                n.IsRead = true;
            }

            public async Task SaveChangesAsync()
                => await context.SaveChangesAsync();
    }

}
