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
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository departmentRepo,
                                 ILogger<DepartmentService> logger)
        {
            _departmentRepo = departmentRepo;
            _logger = logger;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _departmentRepo.GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(long id)
        {
            return await _departmentRepo.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(DepartmentViewModel model, long? createdById)

        {
            try
            {
                var dept = new Department
                {
                    Name = model.Name,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = createdById,
                    Status = ModelStatus.Active
                };
                await _departmentRepo.AddAsync(dept);
                await _departmentRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(DepartmentViewModel model, long? modifiedById)
        {
            try
            {
                var dept = await _departmentRepo.GetByIdAsync(model.Id);
                if (dept == null) return false;

                dept.Name = model.Name;
                dept.ModifiedAt = DateTime.UtcNow;
                dept.ModifiedById = modifiedById;

                await _departmentRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department {Id}", model.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(long id, long? deletedById)
        {
            try
            {
                var dept = await _departmentRepo.GetByIdAsync(id);
                if (dept == null) return false;

                dept.Status = ModelStatus.Deleted;
                dept.ModifiedAt = DateTime.UtcNow;
                dept.ModifiedById = deletedById;

                await _departmentRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {Id}", id);
                return false;
            }
        }
    }
}
