using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class PaymentMethod : CommonFields
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid PaymentMethodId { get; set; }
        [Required]
        public string Description { get; set; }
        public string AccountNumber { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
        public ICollection<InvoiceReceivable> InvoiceReceivables { get; set; }
        public ICollection<BillPayment> BillPayments { get; set; }
    }
}
