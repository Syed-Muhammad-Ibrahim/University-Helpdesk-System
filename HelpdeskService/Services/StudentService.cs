using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskModel.ViewModels.UpdateViewModels;
using HelpdeskRepository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
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
        private readonly AppDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            AppDbContext context,
            ILogger<StudentService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateStudentAsync(StudentRegisterViewModel model, long? createdById)
        {
            try
            {
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

                if (!await _roleManager.RoleExistsAsync("Student"))
                    await _roleManager.CreateAsync(new ApplicationRole { Name = "Student" });

                await _userManager.AddToRoleAsync(user, "Student");

                var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == model.DepartmentId);

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

                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating student.");
                return false;
            }
        }

        public async Task<bool> UpdateStudentAsync(StudentUpdateViewModel model, long? modifiedById)
        {
            try
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == model.Id);

                if (student == null)
                    return false;

                student.Name = model.Name;
                student.Address = model.Address;
                student.Phone = model.Phone;
                student.Status = model.Status;
                student.ModifiedAt = DateTime.UtcNow;
                student.ModifiedById = modifiedById;

                await _context.SaveChangesAsync();
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
            return await _context.Students
                .Include(s => s.User)
                .ToListAsync();
        }
    }

}
