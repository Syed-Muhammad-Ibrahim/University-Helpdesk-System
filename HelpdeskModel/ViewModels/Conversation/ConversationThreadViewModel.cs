using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels.Conversation
{
    public class ConversationThreadViewModel
    {
        public long ComplainId { get; set; }
        public string ComplainDescription { get; set; }
        public bool IsSolved { get; set; }
        public List<ConversationMessageItem> Messages { get; set; }
        public ConversationMessageViewModel NewMessage { get; set; }
    }
}
