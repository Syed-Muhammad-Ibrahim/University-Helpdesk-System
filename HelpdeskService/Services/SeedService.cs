using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskRepository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HelpdeskService.Services
{
    public class SeedService : ISeedService
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SeedService> _logger;

        public SeedService(
            AppDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<SeedService> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public void DatabaseSeeder()
        {
            try
            {
                _logger.LogInformation("Ensuring the database is created.");
                _context.Database.EnsureCreated();

                _logger.LogInformation("Seeding roles.");
                AddRole("Admin");
                AddRole("User");
                AddRole("Student");
                AddRole("Staff");

                // Admin
                _logger.LogInformation("Seeding admin user.");
                var adminEmail = "admin@iubat.edu";
                if (_userManager.FindByEmailAsync(adminEmail).Result == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        FullName = "Md Ibrahim",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = _userManager.CreateAsync(adminUser, "Admin@123").Result;
                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(adminUser, "Admin").Wait();
                    }
                    else
                    {
                        _logger.LogError(
                            "Failed to create admin user: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                // Student
                _logger.LogInformation("Seeding student user.");
                var studentEmail = "student@iubat.edu";
                if (_userManager.FindByEmailAsync(studentEmail).Result == null)
                {
                    var studentUser = new ApplicationUser
                    {
                        FullName = "Default Student",
                        UserName = studentEmail,
                        Email = studentEmail,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var studentResult = _userManager.CreateAsync(studentUser, "Student123!").Result;
                    if (studentResult.Succeeded)
                    {
                        _userManager.AddToRoleAsync(studentUser, "Student").Wait();
                    }
                }

                // Staff
                _logger.LogInformation("Seeding staff user.");
                var staffEmail = "staff@iubat.edu";
                if (_userManager.FindByEmailAsync(staffEmail).Result == null)
                {
                    var staffUser = new ApplicationUser
                    {
                        FullName = "Default Staff",
                        UserName = staffEmail,
                        Email = staffEmail,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var staffResult = _userManager.CreateAsync(staffUser, "Staff123!").Result;
                    if (staffResult.Succeeded)
                    {
                        _userManager.AddToRoleAsync(staffUser, "Staff").Wait();
                    }
                }
                if (!_context.Departments.Any())
                {
                    var departments = new List<Department>
                    {
                        new Department
                        {
                            Name      = "Accounting",
                            CreatedAt = DateTime.UtcNow,
                            Status    = ModelStatus.Active,
                        },
                        new Department
                        {
                            Name       = "IT",
                            CreatedAt  = DateTime.UtcNow,
                            Status     = ModelStatus.Active
                        },
                        new Department
                        {
                            Name       = "Business Administration",
                            CreatedAt  = DateTime.UtcNow,
                            Status     = ModelStatus.Active
                        }

                    };

                    _context.Departments.AddRange(departments);
                    _context.SaveChanges();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private void AddRole(string roleName)
        {
            if (!_roleManager.RoleExistsAsync(roleName).Result)
            {
                var result = _roleManager.CreateAsync(new ApplicationRole { Name = roleName }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(
                        "Failed to create role '" + roleName + "': " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
