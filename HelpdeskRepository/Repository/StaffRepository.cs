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
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Staff?> GetByIdAsync(long id)
        {
            return await _context.Staffs
                .Include(s => s.Department)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            return await _context.Staffs
                .Include(s => s.Department)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task AddAsync(Staff staff)
        {
            await _context.Staffs.AddAsync(staff);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<List<Staff>> GetByDepartmentIdAsync(long departmentId)
        {
            return await _context.Staffs
                .Include(s => s.User)
                .Where(s => s.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
}
