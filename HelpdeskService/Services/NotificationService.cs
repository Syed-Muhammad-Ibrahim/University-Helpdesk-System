using HelpdeskModel.Models;
using HelpdeskRepository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IComplainRepository complainRepository;
        private readonly IStaffRepository staffRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<NotificationService> logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IComplainRepository complainRepository,
            IStaffRepository staffRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<NotificationService> logger)
        {
            this.notificationRepository = notificationRepository;
            this.complainRepository = complainRepository;
            this.staffRepository = staffRepository;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task CreateReplyNotificationAsync(long complainId, long senderUserId)
        {
            var complain = await complainRepository.GetByIdAsync(complainId);
            if (complain == null) return;

            var senderUser = await userManager.FindByIdAsync(senderUserId.ToString());
            if (senderUser == null) return;

            var roles = await userManager.GetRolesAsync(senderUser);
            var senderIsStudent = roles.Contains("Student");

            var receiverIds = new List<long>();

            var deptStaff = await staffRepository.GetByDepartmentIdAsync(complain.DepartmentId);
            receiverIds.AddRange(deptStaff.Select(s => s.UserId));

            var admins = await userManager.GetUsersInRoleAsync("Admin");
            receiverIds.AddRange(admins.Select(a => a.Id));

            if (!senderIsStudent)
                receiverIds.Add(complain.CreatedById);

            receiverIds = receiverIds
                .Where(id => id != senderUserId)
                .Distinct()
                .ToList();

            if (!receiverIds.Any()) return;

            var items = receiverIds.Select(uid => new Notification
            {
                UserId = uid,
                ComplainId = complain.Id,
                Message = $"New reply on complaint #{complain.Id}.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await notificationRepository.AddRangeAsync(items);
            await notificationRepository.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUnreadAsync(long userId)
            => await notificationRepository.GetUnreadByUserIdAsync(userId);

        public async Task MarkAsReadAsync(long notificationId, long userId)
        {
            await notificationRepository.MarkAsReadAsync(notificationId, userId);
            await notificationRepository.SaveChangesAsync();
        }
    }
}
