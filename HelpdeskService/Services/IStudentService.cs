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
    public interface IStudentService
    {
        Task<OperationResult> CreateStudentAsync(StudentRegisterViewModel model, long? currentUserId);
        Task<bool> UpdateStudentAsync(StudentEditViewModel model, long? modifiedById);
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(long id);
        Task<Student?> GetStudentByUserIdAsync(long userId);
        Task<bool> SoftDeleteStudentAsync(long studentId, long? deletedById);

    }
}
