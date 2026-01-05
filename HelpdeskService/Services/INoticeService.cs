using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public interface INoticeService
    {
        Task<bool> CreateNoticeAsync(NoticeViewModel model, long userId, bool isAdmin);
        Task<List<Notice>> GetAllApprovedAsync();
        Task<List<Notice>> GetApprovedByDepartmentAsync(long departmentId);
        Task<List<Notice>> GetPendingNoticesAsync();
        Task<Notice?> GetByIdAsync(long id);
        Task<bool> UpdateNoticeAsync(NoticeViewModel model, long userId, bool isAdmin);
        Task<bool> DeleteNoticeAsync(long id, long userId, bool isAdmin);
        Task<bool> ApproveAsync(long id, long adminId);
        Task<bool> RejectAsync(long id, long adminId);
        Task<List<Notice>> GetAllAsync();
        Task<List<Notice>> GetStaffNoticesAsync(long staffUserId);


    }
}
