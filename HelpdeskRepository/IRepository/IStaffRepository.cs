using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.IRepository
{
    public interface IStaffRepository
    {
        Task<Staff?> GetByIdAsync(long id);
        Task<List<Staff>> GetAllAsync();
        Task AddAsync(Staff staff);
        Task SaveChangesAsync();
    }
}
