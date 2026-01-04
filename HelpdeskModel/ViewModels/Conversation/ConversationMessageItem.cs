using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels.Conversation
{
    public class ConversationMessageItem
    {
        public string SenderName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
    }
}
