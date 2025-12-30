using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.IRepository
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(long id);
        Task<List<Department>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
