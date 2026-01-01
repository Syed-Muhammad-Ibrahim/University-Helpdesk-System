using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskModel.ViewModels.UpdateViewModels;
using HelpdeskRepository.Data;
using HelpdeskRepository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public class StudentService : IStudentService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IStudentRepository studentRepository,
            IDepartmentRepository departmentRepository,
            AppDbContext context,
            ILogger<StudentService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _studentRepository = studentRepository;
            _departmentRepository = departmentRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateStudentAsync(StudentRegisterViewModel model, long? createdById)
        {
            try
            {
                // Identity user create
                var user = new ApplicationUser
                {
                    FullName = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create student user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                // Role assign
                if (!await _roleManager.RoleExistsAsync("Student"))
                    await _roleManager.CreateAsync(new ApplicationRole { Name = "Student" });

                await _userManager.AddToRoleAsync(user, "Student");

                var department = await _departmentRepository.GetByIdAsync(model.DepartmentId);

                if (department == null)
                {
                    _logger.LogError("Invalid DepartmentId {DepartmentId} for student {Email}",
                        model.DepartmentId, model.Email);
                    return false;
                }

                var student = new Student
                {
                    Name = model.Name,
                    User = user,
                    Address = model.Address,
                    Phone = model.Phone,
                    Department = department,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    CreatedById = createdById
                };

                await _studentRepository.AddAsync(student);
                await _studentRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating student.");
                return false;
            }
        }

        public async Task<bool> UpdateStudentAsync(StudentEditViewModel model, long? modifiedById)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(model.Id);
                if (student == null)
                    return false;

                student.Name = model.Name;
                student.Address = model.Address;
                student.Phone = model.Phone;
               
                

                var dept = await _departmentRepository.GetByIdAsync(model.DepartmentId);

                if (dept != null)
                    student.Department = dept;

                student.Status = model.Status;
                student.ModifiedAt = DateTime.UtcNow;
                student.ModifiedById = modifiedById;

                await _studentRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating student {Id}", model.Id);
                return false;
            }
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(long id)
        {
            return await _studentRepository.GetByIdAsync(id);
        }

    }

}