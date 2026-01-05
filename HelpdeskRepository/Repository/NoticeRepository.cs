using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskRepository.Data;
using HelpdeskRepository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskRepository.Repository
{
    public class NoticeRepository : INoticeRepository
    {
        private readonly AppDbContext _context;

        public NoticeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notice notice)
        {
            await _context.Notices.AddAsync(notice);
        }

        public async Task<Notice?> GetByIdAsync(long id)
        {
            return await _context.Notices
                .Include(n => n.Department)
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<Notice>> GetAllAsync()
        {
            return await _context.Notices
                .Include(n => n.Department)
                .Include(n => n.CreatedBy)
                .ToListAsync();
        }

        public async Task<List<Notice>> GetApprovedAsync()
        {
            return await _context.Notices
                .Include(n => n.Department)
                .Include(n => n.CreatedBy)
                .Where(n => n.isApproved && n.Status == ModelStatus.Active)
                .ToListAsync();
        }

        public async Task<List<Notice>> GetApprovedByDepartmentAsync(long departmentId)
        {
            return await _context.Notices
                .Include(n => n.Department)
                .Include(n => n.CreatedBy)
                .Where(n => n.DepartmentId == departmentId &&
                            n.isApproved &&
                            n.Status == ModelStatus.Active)
                .ToListAsync();
        }

        public async Task<List<Notice>> GetPendingAsync()
        {
            return await _context.Notices
                .Include(n => n.Department)
                .Include(n => n.CreatedBy)
                .Where(n => !n.isApproved && n.Status == ModelStatus.Active)
                .ToListAsync();
        }

        public async Task<List<Notice>> GetByCreatorAsync(long createdById)
        {
            return await _context.Notices
                .Include(n => n.Department)
                .Where(n => n.CreatedById == createdById)
                .ToListAsync();
        }

        public void Remove(Notice notice)
        {
            _context.Notices.Remove(notice);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
