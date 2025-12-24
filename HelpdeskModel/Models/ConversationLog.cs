using HelpdeskModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.Models
{
    public class ConversationLog
    {
        public long Id { get; set; }
        public long? CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } 
        public ModelStatus Status { get; set; }
        public long? ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public long? AttachmentId { get; set; }
        public Attachment Attachment { get; set; }
        public string Description {  get; set; }
       }
}
