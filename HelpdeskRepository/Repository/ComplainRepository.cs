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
    public class ComplainRepository : IComplainRepository
    {
        private readonly AppDbContext _context;
        public ComplainRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Complain complain)
            => await _context.Complains.AddAsync(complain);

        public async Task<Complain?> GetByIdAsync(long id)
        {
            return await _context.Complains
                .Include(c => c.Department)
                .Include(c => c.CreatedBy)
                .Include(c => c.Attachment)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Complain>> GetAllAsync()
            =>  await _context.Complains
                .Include(c => c.CreatedBy)
                .Include(c => c.Department)
                .ToListAsync();

        public async Task<List<Complain>> GetByStudentIdAsync(long studentUserId)
            => await _context.Complains
            .Include(c=> c.Department)
                .Where(c => c.CreatedById == studentUserId)
                .ToListAsync();

        public async Task<List<Complain>> GetByDepartmentIdAsync(long departmentId)
            =>  await _context.Complains
                .Include(c => c.CreatedBy)
                .Include(c => c.Department)
                .Where(c => c.DepartmentId == departmentId)
                .ToListAsync();

        public void Remove(Complain complain)
            => _context.Complains.Remove(complain);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<List<Complain>> GetByIdsAsync(List<long> ids)
        {
            return await _context.Complains
                .Include(c => c.Department)
                .Include(c => c.CreatedBy)
                .Include(c => c.Attachment)
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }
    }

}
