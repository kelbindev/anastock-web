using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class Project : CommonFields
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public Guid ProjectId { get; set; }
        [Display(Name = "Project Number")]
        [Required]
        public string ProjectNo { get; set; }
        [Display(Name = "Project Title")]
        [Required, MaxLength(120)]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Installation Date")]
        [Required]
        public DateTime InstallationDate { get; set; }
        [Display(Name = "Installation Time")]
        //[Required]
        public string InstallationTime { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Handover Date")]
        public DateTime? HandoverDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Dismantle Date")]
        public DateTime? DismantleDate { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Target Sales")]
        [Required]
        public decimal TargetSales { get; set; }
        [Display(Name = "Project Budget")]
        [Required]
        public decimal ProjectBudget { get; set; }
        [MaxLength(250)]
        public string Remarks { get; set; }
        public bool InUse { get; set; }
        [Display(Name = "Quote No")]
        public string QuoteNo { get; set; }

        //Relationship
        public Quote Quote { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
    }
}
