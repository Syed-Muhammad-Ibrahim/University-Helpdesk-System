using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public interface IComplainService
    {
        Task<bool> CreateComplainAsync(ComplainViewModel model, long studentUserId);
        Task<List<Complain>> GetStudentComplainsAsync(long studentUserId);
        Task<List<Complain>> GetDepartmentComplainsForStaffAsync(long staffUserId);
        Task<bool> MarkSolvedAsync(long complainId, long actorUserId, bool isAdminOrStaff);
        Task<Complain?> GetByIdForStudentAsync(long complainId, long studentUserId);
        Task<Complain?> GetByIdForStaffAsync(long complainId, long staffUserId, bool isAdmin);
        Task<bool> UpdateComplainAsync(ComplainViewModel model, long studentUserId);
        Task<bool> DeleteComplainAsync(long complainId, long studentUserId);
    }
}
