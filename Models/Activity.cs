using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Anastock.Code.Common;

namespace Anastock.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityId { get; set; }
        public Modules Modules { get; set; }
        public Guid ModuleId { get; set; }
        public string ModuleDescription { get; set; }
        public ActivityType ActivityType { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }

        //Relationship
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }

    }
}
