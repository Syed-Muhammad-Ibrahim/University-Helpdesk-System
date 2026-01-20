using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskModel.ViewModels.UpdateViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public interface IStaffService
    {
        Task<bool> CreateStaffAsync(StaffRegisterViewModel model,long? createdById);
        Task<bool> UpdateStaffAsync(StaffUpdateViewModel model,long? modifiedById);
        Task<List<Staff>> GetAllStaffAsync();
        Task<Staff?> GetStaffByIdAsync(long id);
        Task<bool> UpdateOwnProfileAsync(StaffUpdateViewModel model, long userId);
        Task<Staff> GetStaffByUserIdAsync(long userId);
        Task<bool> SoftDeleteStaffAsync(long staffId, long? deletedById);


    }
}
