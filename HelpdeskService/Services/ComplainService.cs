using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskRepository.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{

    public class ComplainService : IComplainService
    {
        private readonly IComplainRepository _complainRepository;
        private readonly IStudentService _studentService;
        private readonly IStaffService _staffService;
        private readonly ILogger _logger;

        public ComplainService(
            IComplainRepository complainRepository,
            IStudentService studentService,
            IStaffService staffService,
            ILogger<ComplainService> logger)
        {
            _complainRepository = complainRepository;
            _studentService = studentService;
            _staffService = staffService;
            _logger = logger;
        }

        //Create Complain
        public async Task<bool> CreateComplainAsync(ComplainViewModel model, long studentUserId)
        {
            try
            {
                var student = await _studentService.GetStudentByUserIdAsync(studentUserId);
                if (student == null) return false;

                var complain = new Complain
                {
                    Description = model.Description,
                    DepartmentId = model.DepartmentId,
                    AttachmentId = model.AttachmentId,
                    CreatedById = studentUserId,
                    StudentId = student.Id,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    isSolved = false
                };

                await _complainRepository.AddAsync(complain);
                await _complainRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating complain for user {UserId}", studentUserId);
                return false;
            }
        }

        //Update Complain
        public async Task<bool> UpdateComplainAsync(ComplainViewModel model, long studentUserId)
        {
            try
            {
                var complain = await _complainRepository.GetByIdAsync(model.Id);
                if (complain == null) return false;

                if (complain.CreatedById != studentUserId) return false; 
                if (complain.isSolved) return false;

                complain.Description = model.Description;
                complain.DepartmentId = model.DepartmentId;
                complain.AttachmentId = model.AttachmentId;
                complain.ModifiedAt = DateTime.UtcNow;
                complain.ModifiedById = studentUserId;

                await _complainRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complain {Id}", model.Id);
                return false;
            }
        }

        // DELETE Complain
        public async Task<bool> DeleteComplainAsync(long complainId, long studentUserId)
        {
            try
            {
                var complain = await _complainRepository.GetByIdAsync(complainId);
                if (complain == null) return false;

                if (complain.CreatedById != studentUserId) return false;
                if (complain.isSolved) return false;

                _complainRepository.Remove(complain);
                await _complainRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting complain {Id}", complainId);
                return false;
            }
        }

        //Student Complain List
        public async Task<List<Complain>> GetStudentComplainsAsync(long studentUserId)
        { 
            return await _complainRepository.GetByStudentIdAsync(studentUserId); 
        }

        // Complain List for Staff
        public async Task<List<Complain>> GetDepartmentComplainsForStaffAsync(long staffUserId)
        {
            var staff = await _staffService.GetStaffByUserIdAsync(staffUserId);
            if (staff == null) return new List<Complain>();

            return await _complainRepository.GetByDepartmentIdAsync(staff.DepartmentId);
        }

        // Mark Solve
        public async Task<bool> MarkSolvedAsync(long complainId, long actorUserId, bool isAdminOrStaff)
        {
            try
            {
                var complain = await _complainRepository.GetByIdAsync(complainId);
                if (complain == null) return false;

                if (!isAdminOrStaff) return false;

                complain.isSolved = true;
                complain.Status = ModelStatus.InActive;
                complain.ModifiedAt = DateTime.UtcNow;
                complain.ModifiedById = actorUserId;

                await _complainRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking complain {Id} solved", complainId);
                return false;
            }
        }

        // single complain for student
        public async Task<Complain?> GetByIdForStudentAsync(long complainId, long studentUserId)
        {
            var complain = await _complainRepository.GetByIdAsync(complainId);
            if (complain == null) return null;

            
            return complain.CreatedById == studentUserId ? complain : null;
        }

        // single complain for staff/admin
        public async Task<Complain?> GetByIdForStaffAsync(long complainId, long staffUserId, bool isAdmin)
        {
            var complain = await _complainRepository.GetByIdAsync(complainId);
            if (complain == null) return null;

            if (isAdmin) return complain;

            var staff = await _staffService.GetStaffByUserIdAsync(staffUserId);
            if (staff == null) return null;

            return complain.DepartmentId == staff.DepartmentId ? complain : null;
        }

        // All Complain For Admin
        public async Task<List<Complain>> GetAllComplainsAsync()
        {
            return await _complainRepository.GetAllAsync();
        }
    }

