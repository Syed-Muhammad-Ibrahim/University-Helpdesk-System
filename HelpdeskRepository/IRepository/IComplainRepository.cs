using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.IRepository
{
    public interface IComplainRepository
    {
        Task AddAsync(Complain complain);
        Task<Complain?> GetByIdAsync(long id);
        Task<List<Complain>> GetAllAsync();
        Task<List<Complain>> GetByStudentIdAsync(long studentUserId);
        Task<List<Complain>> GetByDepartmentIdAsync(long departmentId);
        void Remove(Complain complain);
        Task SaveChangesAsync();
        Task<List<Complain>> GetByIdsAsync(List<long> ids);
    }
}
