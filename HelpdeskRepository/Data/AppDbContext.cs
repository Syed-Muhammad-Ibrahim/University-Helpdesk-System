using HelpdeskModel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationLog> ConversationLogs { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<NoticeLog>NoticeLogs { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<ComplainLog>(entity =>
        //    {
        //        // Created By
        //        entity.HasOne(x => x.CreatedBy)
        //            .WithMany()
        //            .HasForeignKey(x => x.CreatedById)
        //            .OnDelete(DeleteBehavior.Restrict);

        //        // Approved By
        //        entity.HasOne(x => x.ApprovedBy)
        //            .WithMany()
        //            .HasForeignKey(x => x.ApprovedById)
        //            .OnDelete(DeleteBehavior.Restrict);

        //        // Rejected By
        //        entity.HasOne(x => x.RejectedBy)
        //            .WithMany()
        //            .HasForeignKey(x => x.RejectedById)
        //            .OnDelete(DeleteBehavior.Restrict);

        //        // Complain
        //        entity.HasOne(x => x.Complain)
        //            .WithMany()
        //            .HasForeignKey("ComplainId")
        //            .OnDelete(DeleteBehavior.Restrict);

        //        // Optional: Department
        //        entity.HasOne(x => x.Department)
        //            .WithMany()
        //            .OnDelete(DeleteBehavior.Restrict);

        //        // Optional: Attachment
        //        entity.HasOne(x => x.Attachment)
        //            .WithMany()
        //            .OnDelete(DeleteBehavior.Restrict);

        //        builder.Entity<Conversation>()
        //            .HasOne(c => c.User)
        //            .WithMany()
        //            .HasForeignKey(c => c.UserId)
        //            .OnDelete(DeleteBehavior.Restrict);   // or DeleteBehavior.NoAction

        //        builder.Entity<Conversation>()
        //            .HasOne(c => c.CreatedBy)
        //            .WithMany()
        //            .HasForeignKey(c => c.CreatedById)
        //            .OnDelete(DeleteBehavior.Restrict);

        //        builder.Entity<Conversation>()
        //            .HasOne(c => c.ModifiedBy)
        //            .WithMany()
        //            .HasForeignKey(c => c.ModifiedById)
        //            .OnDelete(DeleteBehavior.Restrict);

        //        builder.Entity<Conversation>()
        //            .HasOne(c => c.Complain)
        //            .WithMany()
        //            .HasForeignKey(c => c.ComplainId)
        //            .OnDelete(DeleteBehavior.Restrict);

        //        builder.Entity<Conversation>()
        //            .HasOne(c => c.Attachment)
        //            .WithMany()
        //            .HasForeignKey(c => c.AttachmentId)
        //            .OnDelete(DeleteBehavior.Restrict);

        //        builder.Entity<Student>()
        //            .HasIndex(s => s.StudentId)
        //            .IsUnique();
        //    });




        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var fk in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }


    }
}
