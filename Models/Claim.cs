using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class Claim : CommonFields
    {
        [Key]
        [Required]
        public Guid ClaimId { get; set; }
        [Required, MaxLength(20)]
        [DisplayName("Claim No")]
        public string ClaimNo { get; set; }
        [Required]
        [DisplayName("Expense Date From")]
        [DataType(DataType.Date)]
        public DateTime ExpenseDateFrom { get; set; }
        [Required]
        [DisplayName("Expense Date To")]
        [DataType(DataType.Date)]
        public DateTime ExpenseDateTo { get; set; }
        public int ExpenseType { get; set; }
        [MaxLength(20)]
        public string? Status { get; set; }
        [DisplayName("Claim Amount")]
        public decimal SubTotal { get; set; }
        [DisplayName("GST Amount")]
        public decimal Tax { get; set; }
        [DisplayName("Net Amount")]
        public decimal Total { get; set; }
        public string? Remarks { get; set; }
        [Required]
        public byte[] Attachment { get; set; }
        public string AttachmentExtension { get; set; }

        //Relationship
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
    }
}
