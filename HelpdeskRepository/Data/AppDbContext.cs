using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HelpdeskModel.Models;

namespace HelpdeskRepository.Data
{
    public class AppDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Complain> Complains { get; set; }
        public DbSet<ComplainLog> ComplainsLog { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ComplainLog>(entity =>
            {
                // Created By
                entity.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Approved By
                entity.HasOne(x => x.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(x => x.ApprovedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Rejected By
                entity.HasOne(x => x.RejectedBy)
                    .WithMany()
                    .HasForeignKey(x => x.RejectedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Complain
                entity.HasOne(x => x.Complain)
                    .WithMany()
                    .HasForeignKey("ComplainId")
                    .OnDelete(DeleteBehavior.Restrict);

                // Optional: Department
                entity.HasOne(x => x.Department)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

                // Optional: Attachment
                entity.HasOne(x => x.Attachment)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
