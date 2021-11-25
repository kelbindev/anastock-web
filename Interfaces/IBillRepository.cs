using Anastock.Models;
using Anastock.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Anastock.Interfaces
{
    public interface IBillRepository
    {
        Bill GetBill(Guid id);
        IEnumerable<BillDetails> GetBillDetails(Guid id);
        VendorAddress getVendorAddress(int id);
        IEnumerable<Bill> GetAllBills();
        IEnumerable<Bill> GetBillsByCompanyId(int id);
        IEnumerable<BillViewModels> GetBillsWithPOByCompanyId(int id);
        Bill Create(Bill model, int companyId);
        bool Update(Bill model, int companyId);
        bool Delete(Guid id, int companyId);

        bool CreatePayment(BillPaymentViewModel model, int companyId);
        List<BillPaymentViewModel> GetBillPayments(int companyId);
    }
}
