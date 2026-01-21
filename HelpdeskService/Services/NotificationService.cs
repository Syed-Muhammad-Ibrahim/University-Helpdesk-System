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
        private readonly INotificationRepository _notificationRepository;
        private readonly IComplainRepository _complainRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IComplainRepository complainRepository,
            IStaffRepository staffRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _complainRepository = complainRepository;
            _staffRepository = staffRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task CreateReplyNotificationAsync(long complainId, long senderUserId)
        {
            try
            {
                var complain = await _complainRepository.GetByIdAsync(complainId);
                if (complain == null) return;

                var senderUser = await _userManager.FindByIdAsync(senderUserId.ToString());
                if (senderUser == null) return;

                var roles = await _userManager.GetRolesAsync(senderUser);
                var senderIsStudent = roles.Contains("Student");

                // receivers: dept all staff + all admin
                var receiverIds = new List<long>();

                var deptStaff = await _staffRepository.GetByDepartmentIdAsync(complain.DepartmentId);
                receiverIds.AddRange(deptStaff.Select(s => s.UserId));

                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                receiverIds.AddRange(admins.Select(a => a.Id));

                // staff/admin reply 
                if (!senderIsStudent)
                    receiverIds.Add(complain.CreatedById);

                receiverIds = receiverIds
                    .Where(id => id != senderUserId)
                    .Distinct()
                    .ToList();

                foreach (var uid in receiverIds)
                {
                    var existing = await _notificationRepository.GetByUserAndComplainAsync(uid, complain.Id);

                    if (existing == null)
                    {
                        var n = new Notification
                        {
                            UserId = uid,
                            ComplainId = complain.Id,
                            IsRead = false,
                            UnreadCount = 1,
                            LastAt = DateTime.UtcNow,
                            LastMessage = $"New reply on complaint #{complain.Id}.",
                            Message = $"New reply on complaint #{complain.Id}."
                        };
                        await _notificationRepository.AddAsync(n);
                    }
                    else
                    {
                        existing.IsRead = false;
                        existing.UnreadCount += 1;
                        existing.LastAt = DateTime.UtcNow;
                        existing.LastMessage = $"New reply on complaint #{complain.Id}.";
                        existing.Message = existing.LastMessage;
                    }
                }

                await _notificationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/updating notifications for complainId {ComplainId}", complainId);
            }
        }

        public async Task<List<Notification>> GetUnreadAsync(long userId)
            => await _notificationRepository.GetUnreadByUserIdAsync(userId);


        public async Task<long?> OpenNotificationAsync(long notificationId, long userId)
        {
            var n = await _notificationRepository.GetByIdForUserAsync(notificationId, userId);
            if (n == null) return null;

            n.IsRead = true;
            n.UnreadCount = 0;

            await _notificationRepository.SaveChangesAsync();
            return n.ComplainId;
        }

        public async Task ClearMyComplainNotificationAsync(long userId, long complainId)
        {
            var n = await _notificationRepository.GetByUserAndComplainAsync(userId, complainId);
            if (n == null) return;

            n.IsRead = true;
            n.UnreadCount = 0;

            await _notificationRepository.SaveChangesAsync();
        }
    }
}
