using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskModel.ViewModels.UpdateViewModels;
using HelpdeskRepository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HelpdeskService.Services
{
    public class StaffService : IStaffService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly ILogger<StaffService> _logger;
        

        public StaffService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            AppDbContext context,
            ILogger<StaffService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateStaffAsync(StaffRegisterViewModel model, long? createdById)
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
                // Getting The Department
                var department = await _context.Departments.
                    FirstOrDefaultAsync(d => d.Id == model.DepartmentId);
                if (department == null)
                {
                    _logger.LogError("Invalid DepartmentId {DepartmentId} for staff {Email}",
                        model.DepartmentId, model.Email);
                    return false;
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create staff user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                // Role ensure + assign
                if (!await _roleManager.RoleExistsAsync("Staff"))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = "Staff" });
                }

                await _userManager.AddToRoleAsync(user, "Staff");

                // Staff table e entry
                var staff = new Staff
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
                


                _context.Staffs.Add(staff);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating staff.");
                return false;
            }
        }

        public async Task<bool> UpdateStaffAsync(StaffUpdateViewModel model, long? modifiedById)
        {
            try
            {
                var staff = await _context.Staffs
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == model.DepartmentId);

                if (staff == null)
                {
                    return false;
                }

                staff.Name = model.Name;
                staff.Address = model.Address;
                staff.Phone = model.Phone; ;
                var dept = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == model.DepartmentId);

                if (dept == null)
                {
                    staff.Department = dept;
                }
                    staff.ModifiedAt = DateTime.UtcNow;
                    staff.ModifiedById = modifiedById;

                    await _context.SaveChangesAsync();

                    return true;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating staff {Id}", model.Id);

                return false;
            }
        }

        public async Task<bool> DeleteStaffAsync(long id, long? deletedById)
        {
            try
            {
                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (staff == null)
                    return false;

                staff.Status = ModelStatus.Deleted;
                staff.ModifiedAt = DateTime.UtcNow;
                staff.ModifiedById = deletedById;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while soft deleting staff {Id}", id);
                return false;
            }
        }

    }
}
