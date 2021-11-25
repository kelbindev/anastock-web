using Anastock.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IInvoiceReceivableRepository
    {
        bool Create(InvoiceReceivableViewModel model, int companyId);
        List<InvoiceReceivableViewModel> GetInvoiceReceivables(int companyId);
    }
}
