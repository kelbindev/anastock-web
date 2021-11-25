using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class Customer : CommonFields
    {
        [Key]
        [Required]
        public Guid CustomerId { get; set; }
        [Required, MaxLength(100)]
        public string CustomerName { get; set; }
        [Required, MaxLength(200)]
        public string CustomerEmail { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        [MaxLength(50)]
        public string? Website { get; set; }
        [Required]
        public bool isActive { get; set; }
        //Relationship
        public ICollection<CustomerAddress> customerAddresses { get; set; }
        public ICollection<Quote> Quotes { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Project> Projects { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
    }
}
