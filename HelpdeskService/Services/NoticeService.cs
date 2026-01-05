using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskRepository.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public class NoticeService : INoticeService
    {
        private readonly INoticeRepository _noticeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger _logger;

        public NoticeService(
            INoticeRepository noticeRepository,
            IDepartmentRepository departmentRepository,
            ILogger<NoticeService> logger)
        {
            _noticeRepository = noticeRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        // CREATE: Admin + Staff
        public async Task<bool> CreateNoticeAsync(NoticeViewModel model, long userId, bool isAdmin)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(model.DepartmentId);
                if (department == null)
                {
                    _logger.LogError("Invalid DepartmentId {DepartmentId} for notice", model.DepartmentId);
                    return false;
                }

                var notice = new Notice
                {
                    Description = model.Description,
                    DepartmentId = model.DepartmentId,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    AttachmentId = model.AttachmentId ?? 0 
                };

                if (isAdmin)
                {
                    notice.isApproved = true;
                    notice.ApprovedById = userId;
                    notice.ApprovedAt = DateTime.UtcNow;
                }
                else
                {
                    notice.isApproved = false;
                }

                await _noticeRepository.AddAsync(notice);
                await _noticeRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notice for user {UserId}", userId);
                return false;
            }
        }

        public async Task<List<Notice>> GetAllApprovedAsync()
            => await _noticeRepository.GetApprovedAsync();

        public async Task<List<Notice>> GetApprovedByDepartmentAsync(long departmentId)
            => await _noticeRepository.GetApprovedByDepartmentAsync(departmentId);

        public async Task<List<Notice>> GetPendingNoticesAsync()
            => await _noticeRepository.GetPendingAsync();

        public async Task<Notice?> GetByIdAsync(long id)
            => await _noticeRepository.GetByIdAsync(id);

        public async Task<bool> UpdateNoticeAsync(NoticeViewModel model, long userId, bool isAdmin)
        {
            try
            {
                var notice = await _noticeRepository.GetByIdAsync(model.Id);
                if (notice == null) return false;

                if (!isAdmin)
                {
                    if (notice.CreatedById != userId) return false;
                    if (notice.isApproved) return false;
                }

                var department = await _departmentRepository.GetByIdAsync(model.DepartmentId);
                if (department == null) return false;

                notice.Description = model.Description;
                notice.DepartmentId = model.DepartmentId;
                notice.AttachmentId = model.AttachmentId ?? notice.AttachmentId;
                notice.ModifiedAt = DateTime.UtcNow;
                notice.ModifiedById = userId;

                await _noticeRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notice {Id}", model.Id);
                return false;
            }
        }

        public async Task<bool> DeleteNoticeAsync(long id, long userId, bool isAdmin)
        {
            try
            {
                var notice = await _noticeRepository.GetByIdAsync(id);
                if (notice == null) return false;

                if (!isAdmin)
                {
                    if (notice.CreatedById != userId) return false;
                    if (notice.isApproved) return false;
                }

                _noticeRepository.Remove(notice);
                await _noticeRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notice {Id}", id);
                return false;
            }
        }

        public async Task<bool> ApproveAsync(long id, long adminId)
        {
            try
            {
                var notice = await _noticeRepository.GetByIdAsync(id);
                if (notice == null) return false;

                notice.isApproved = true;
                notice.Status = ModelStatus.Active;
                notice.ApprovedById = adminId;
                notice.ApprovedAt = DateTime.UtcNow;

                await _noticeRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving notice {Id}", id);
                return false;
            }
        }

        public async Task<bool> RejectAsync(long id, long adminId)
        {
            try
            {
                var notice = await _noticeRepository.GetByIdAsync(id);
                if (notice == null) return false;

                notice.isApproved = false;
                notice.Status = ModelStatus.InActive;
                notice.RejectedById = adminId;
                notice.RejectedAt = DateTime.UtcNow;

                await _noticeRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting notice {Id}", id);
                return false;
            }
        }
    }
}
