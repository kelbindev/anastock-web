using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    [Table("Company")]
    public class CompanyViewModel
    {
        [Key]
        public int CompanyId { get; set; }

        [Display(Name = "Company Name")]
        public string Name { get; set; }

        public decimal GST { get; set; }

        [Display(Name = "Company Address")]
        public string Address { get; set; }
        [Display(Name = "Company Email")]
        public string Email { get; set; }
        [Display(Name = "Company Phone")]
        public string Phone { get; set; }
        [Display(Name = "Company Fax")]
        public string Fax { get; set; }
        [Display(Name = "Company Website")]
        public string Website { get; set; }
        [Display(Name = "GST Enable")]
        public bool IsGSTEnable { get; set; }
        [Display(Name = "GST Registration No")]
        public string GSTRegNo { get; set; }
        [Display(Name = "Company Logo")]
        public byte[] Logo { get; set; }
        public string LogoExtension { get; set; }
        //Relationship
        public ICollection<ProductAndService> ProductAndService { get; set; }
        public ICollection<Customer> Customer { get; set; }
        public ICollection<Vendor> Vendor { get; set; }
        public ICollection<Quote> Quote { get; set; }
        public ICollection<Invoice> Invoice { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrder { get; set; }
        public ICollection<Bill> Bills{ get; set; }
        public ICollection<ApplicationUser> ApplicationUser { get; set; }
        public ICollection<Project> Project { get; set; }
        public ICollection<Claim> Claim { get; set; }
        public ICollection<Activity> Activity { get; set; }
    }
}
