using HelpdeskRepository.Data;
using Microsoft.AspNetCore.Identity;
using Student_Complain_Management_System.Models;
using HelpdeskModel.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HelpdeskService.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger =
                scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                logger.LogInformation("Ensuring the database is created.");
                await context.Database.EnsureCreatedAsync();

                // Seed roles
                logger.LogInformation("Seeding roles.");

                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");
                await AddRoleAsync(roleManager, "Student");
                await AddRoleAsync(roleManager, "Staff");


                // Seed admin user
                logger.LogInformation("Seeding admin user.");
                var adminEmail = "admin@iubat.edu";

                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        FullName = "Md Ibrahim",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");

                    if (result.Succeeded)
                    {
                        logger.LogInformation("Assigning Admin role to admin user.");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError(
                            "Failed to create admin user: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                // Seed student user
                logger.LogInformation("Seeding admin user.");
                var studentEmail = "student@iubat.edu";
                if (await userManager.FindByEmailAsync(studentEmail) == null)
                {
                    var studentUser = new ApplicationUser
                    {
                        FullName = "Default Student",
                        UserName = studentEmail,
                        Email = studentEmail,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var studentResult = await userManager.CreateAsync(studentUser, "Student123!");
                    if (studentResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(studentUser, "Student");
                    }
                }

                // Seed staff user
                logger.LogInformation("Seeding admin user.");
                var staffEmail = "staff@iubat.edu";
                if (await userManager.FindByEmailAsync(staffEmail) == null)
                {
                    var staffUser = new ApplicationUser
                    {
                        FullName = "Default Staff",
                        UserName = staffEmail,
                        Email = staffEmail,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var staffResult = await userManager.CreateAsync(staffUser, "Staff123!");
                    if (staffResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(staffUser, "Staff");
                    }
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task AddRoleAsync(
            RoleManager<ApplicationRole> roleManager,
            string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(
                    new ApplicationRole { Name = roleName });

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create role '{roleName}': " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
