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

        public async Task<OperationResult> CreateStudentAsync(StudentRegisterViewModel model, long? createdById)
        {
            var res = new OperationResult();
            ApplicationUser? user = null;

            try
            {
                var existing = await _studentRepository.GetByStudentIdAsync(model.StudentId);
                if (existing != null)
                {
                    _logger.LogWarning("Duplicate StudentCode {StudentCode}", model.StudentId);
                    AddError(res, "StudentId", "This Student Id is already taken.");
                    res.Succeeded = false;
                    return res;
                }

                var department = await _departmentRepository.GetByIdAsync(model.DepartmentId);
                if (department == null)
                {
                    _logger.LogError("Invalid DepartmentId {DepartmentId} for student {Email}", model.DepartmentId, model.Email);
                    AddError(res, "DepartmentId", "Please select a valid Department.");
                    res.Succeeded = false;
                    return res;
                }

                user = new ApplicationUser
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

                    var emailDupAdded = false;

                    foreach (var e in result.Errors)
                    {
                        if (!emailDupAdded && (e.Code.Contains("DuplicateEmail") || e.Code.Contains("DuplicateUserName")))
                        {
                            AddError(res, "Email", "The email already exists.");
                            emailDupAdded = true;
                            continue;
                        }

                        AddError(res, "", e.Description);
                    }

                    res.Succeeded = false;
                    return res;
                }

                if (!await _roleManager.RoleExistsAsync("Student"))
                {
                    var roleCreate = await _roleManager.CreateAsync(new ApplicationRole { Name = "Student" });
                    if (!roleCreate.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        AddError(res, "", "Could not create Student role. Please try again.");
                        res.Succeeded = false;
                        return res;
                    }
                }

                var addRole = await _userManager.AddToRoleAsync(user, "Student");
                if (!addRole.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    AddError(res, "", "Registration failed while assigning role. Please try again.");
                    res.Succeeded = false;
                    return res;
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
                    CreatedById = createdById,
                    StudentId = model.StudentId
                };

                await _studentRepository.AddAsync(student);

                try
                {
                    await _studentRepository.SaveChangesAsync();
                }
                catch
                {
                    await _userManager.DeleteAsync(user);
                    throw;
                }

                res.Succeeded = true;
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating student.");

                if (user != null)
                {
                    try { await _userManager.DeleteAsync(user); } catch { }
                }

                AddError(res, "", "Something went wrong. Please try again.");
                res.Succeeded = false;
                return res;
            }
        }

        public async Task<bool> UpdateStudentAsync(StudentEditViewModel model, long? modifiedById)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(model.Id);
                if (student == null) return false;

                student.Name = model.Name;
                student.Address = model.Address;
                student.Phone = model.Phone;
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

        public async Task<Student?> GetStudentByUserIdAsync(long userId)
        {
            return await _studentRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> SoftDeleteStudentAsync(long studentId, long? deletedById)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                {
                    _logger.LogWarning("Student {Id} not found for deletion", studentId);
                    return false;
                }

                student.Status = ModelStatus.Deleted;
                student.ModifiedAt = DateTime.UtcNow;
                student.ModifiedById = deletedById;

                await _studentRepository.SaveChangesAsync();

                _logger.LogInformation("Student {Id} soft deleted by user {UserId}", studentId, deletedById);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while soft deleting student {Id}", studentId);
                return false;
            }
        }

        private static void AddError(OperationResult res, string key, string message)
        {
            if (!res.Errors.ContainsKey(key))
                res.Errors[key] = new List<string>();

            res.Errors[key].Add(message);
        }
    }

}