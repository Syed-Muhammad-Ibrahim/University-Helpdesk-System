using HelpdeskModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.IRepository
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(long id);

        Task<List<Student>> GetAllAsync();

        Task AddAsync(Student student);

        Task SaveChangesAsync();
        Task<Student?> GetByUserIdAsync(long userId);
    }
}
