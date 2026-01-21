using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; } = new();


    }
}
