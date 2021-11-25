using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        PurchaseOrder GetPurchaseOrder(Guid id);
        IEnumerable<PurchaseOrderDetails> GetPurchaseOrderDetails(Guid id);
        VendorAddress getVendorAddress(int id);
        IEnumerable<PurchaseOrder> GetAllPurchaseOrders();
        IEnumerable<PurchaseOrder> GetPurchaseOrdersByCompanyId(int id);
        PurchaseOrder Create(PurchaseOrder model, int companyId);
        bool Update(PurchaseOrder model, int companyId);
        bool Delete(Guid id, int companyId);
        bool Convert(Guid id, int companyId);
        bool CancelPO(Guid id, int companyId);
    }
}
