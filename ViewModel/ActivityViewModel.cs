using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class ActivityViewModel
    {
        public string Module { get; set; }
        public Guid ModuleId { get; set; }
        public string ModuleDescription { get; set; }
        public string ActivityType { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
    }
}
