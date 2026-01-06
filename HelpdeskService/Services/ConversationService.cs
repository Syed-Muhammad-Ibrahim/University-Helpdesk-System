using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels.Conversation;
using HelpdeskRepository.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskService.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IComplainRepository _complainRepository;
        private readonly IStaffService _staffService;
        private readonly IStudentService _studentService;
        private readonly ILogger _logger;

        public ConversationService(
            IConversationRepository conversationRepository,
            IComplainRepository complainRepository,
            IStaffService staffService,
            IStudentService studentService,
            ILogger<ConversationService> logger)
        {
            _conversationRepository = conversationRepository;
            _complainRepository = complainRepository;
            _staffService = staffService;
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<ConversationThreadViewModel?> GetThreadForUserAsync(long complainId, long userId, ClaimsPrincipal user)
        {
            var complain = await _complainRepository.GetByIdAsync(complainId);
            if (complain == null) return null;

            var isAdmin = user.IsInRole("Admin");
            var isStaff = user.IsInRole("Staff");
            var isStudent = user.IsInRole("Student");

            if (isStudent && complain.CreatedById != userId)
                return null;

            if (isStaff && !isAdmin)
            {
                var staff = await _staffService.GetStaffByUserIdAsync(userId);
                if (staff == null || staff.DepartmentId != complain.DepartmentId)
                    return null;
            }

            // ensure conversation exists
            var conversation = await _conversationRepository.GetByComplainIdAsync(complainId);
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    CreatedById = userId,
                    UserId = userId,
                    ComplainId = complainId,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    Description = "Conversation for complain #" + complainId
                };
                await _conversationRepository.AddConversationAsync(conversation);
                await _conversationRepository.SaveChangesAsync();
            }

            var logs = await _conversationRepository.GetLogsByConversationIdAsync(conversation.Id);

            var vm = new ConversationThreadViewModel
            {
                ComplainId = complain.Id,
                ComplainDescription = complain.Description,
                IsSolved = complain.isSolved,
                Messages = new List<ConversationMessageItem>(),
                NewMessage = new ConversationMessageViewModel { ComplainId = complain.Id }
            };

            foreach (var log in logs)
            {
                vm.Messages.Add(new ConversationMessageItem
                {
                    SenderName = log.CreatedBy?.FullName ?? "Unknown",
                    CreatedAt = log.CreatedAt,
                    Text = log.Description
                });
            }

            return vm;
        }

        public async Task<bool> AddMessageAsync(ConversationMessageViewModel model, long userId)
        {
            try
            {
                var complain = await _complainRepository.GetByIdAsync(model.ComplainId);
                if (complain == null || complain.isSolved) return false;

                var conversation = await _conversationRepository.GetByComplainIdAsync(model.ComplainId);
                if (conversation == null)
                    return false;

                var log = new ConversationLog
                {
                    ConversationId = conversation.Id,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    Description = model.Text
                };

                await _conversationRepository.AddLogAsync(log);
                await _conversationRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding message to complain {Id}", model.ComplainId);
                return false;
            }
        }

        public async Task<List<Complain>> GetComplainsRepliedByUserAsync(long userId)
        {
            var logs = await _conversationRepository.GetLogsByUserIdAsync(userId);

            var complainIds = logs
                .Where(l => l.Conversation?.ComplainId != null)
                .Select(l => l.Conversation.ComplainId.Value)
                .Distinct()
                .ToList();

            if (!complainIds.Any())
                return new List<Complain>();

            return await _complainRepository.GetByIdsAsync(complainIds);
        }
    }
}
