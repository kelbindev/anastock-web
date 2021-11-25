using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice(Guid id);
        IEnumerable<InvoiceDetails> GetInvoiceDetails(Guid id);
        CustomerAddress getCustomerAddress(int id);
        IEnumerable<Invoice> GetInvoiceByCompanyId(int id);
        bool Create(Invoice model, int companyId);
        bool Update(Invoice model, int companyId);
        bool Delete(Guid id, int companyId);
    }
}
