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
        Task<bool> DeleteStaffAsync(long id, long? deletedById);

    }
}
