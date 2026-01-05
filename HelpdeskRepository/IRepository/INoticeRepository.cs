using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.IRepository
{
    public interface INoticeRepository
    {
        Task AddAsync(Notice notice);
        Task<Notice?> GetByIdAsync(long id);
        Task<List<Notice>> GetAllAsync();
        Task<List<Notice>> GetApprovedAsync();
        Task<List<Notice>> GetApprovedByDepartmentAsync(long departmentId);
        Task<List<Notice>> GetPendingAsync();
        Task<List<Notice>> GetByCreatorAsync(long createdById);
        void Remove(Notice notice);
        Task SaveChangesAsync();
    }
}
