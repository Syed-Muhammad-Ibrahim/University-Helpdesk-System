using HelpdeskModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public interface IStaffService
    {
        Task<bool> CreateStaffAsync(StaffRegisterViewModel model);
    }
}
