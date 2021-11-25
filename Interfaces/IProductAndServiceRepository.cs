using Anastock.Models;
using Anastock.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IProductAndServiceRepository
    {
        ProductAndService GetProductAndService(Guid id);
        IEnumerable<ProductAndService> GetAllProductAndService(int companyId);
        IEnumerable<ProductAndService> GetProductAndServiceById(int companyId, Guid id);
        bool Update(ProductAndServiceViewModel ProductAndServiceChanges);
        bool Delete(Guid id);
        bool Create(ProductAndServiceViewModel NewProductAndService);
        IEnumerable<ProductBalanceViewModel> getStockBalance(int companyId);
        IEnumerable<ProductBalanceDetailsViewModel> getStockBalanceDetails(Guid productId);
    }
}
