using HelpdeskRepository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize]
    public class AttachmentController : Controller
    {
        private readonly AppDbContext context;

        public AttachmentController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Download(long id)
        {
            var att = await context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
            if (att == null) return NotFound();

            var relative = att.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relative);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            var downloadName = string.IsNullOrWhiteSpace(att.FileNmae) ? "attachment" : att.FileNmae;
            var contentType = string.IsNullOrWhiteSpace(att.FileType) ? "application/octet-stream" : att.FileType;

            return PhysicalFile(physicalPath, contentType, downloadName);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadComplain(long complainId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var complain = await context.Complains
                .Include(c => c.Attachment)
                .FirstOrDefaultAsync(c => c.Id == complainId);

            if (complain == null || complain.Attachment == null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");
            var isStaff = User.IsInRole("Staff");
            var isStudent = User.IsInRole("Student");

            if (isStudent && complain.CreatedById != userId)
                return Forbid();

            if (isStaff)
            {
                var staff = await context.Staffs.FirstOrDefaultAsync(s => s.UserId == userId);
                if (staff == null || complain.DepartmentId != staff.DepartmentId)
                    return Forbid();
            }

            var att = complain.Attachment;
            var relative = att.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relative);

            if (!System.IO.File.Exists(physicalPath)) return NotFound();

            var downloadName = string.IsNullOrWhiteSpace(att.FileNmae) ? "attachment" : att.FileNmae;
            var contentType = string.IsNullOrWhiteSpace(att.FileType) ? "application/octet-stream" : att.FileType;

            return PhysicalFile(physicalPath, contentType, downloadName);
        }
    }
}
