using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Anastock.Code
{
    public class Common
    {
        public enum Modules
        {
            Project = 1,
            Quote = 2,
            Invoice = 3,
            [Display(Name = "Invoice Payment")]
            InvoiceReceivable = 4,
            [Display(Name = "Purchase Order")]
            PurchaseOrder = 5,
            Bill = 6,
            [Display(Name = "Bill Payment")]
            BillPayment = 7,
            [Display(Name = "Product And Service")]
            ProductAndService = 8,
            Customer = 9,
            Vendor = 10,
            [Display(Name = "Payment Method")]
            PaymentMethod = 11,
            Company = 12
        }
        public enum ActivityType
        {
            Create = 1,
            Update = 2,
            Delete = 3,
            Cancel = 4
        }
    }
}
