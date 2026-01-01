using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(long id);
        Task<bool> CreateAsync(DepartmentViewModel model, long? createdById);
        Task<bool> UpdateAsync(DepartmentViewModel model, long? modifiedById);
        Task<bool> DeleteAsync(long id, long? deletedById);
    }
}
