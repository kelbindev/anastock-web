using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IPaymentMethodRepository
    {
        PaymentMethod GetPaymentMethod(Guid id);
        IEnumerable<PaymentMethod> GetAllPaymentMethod(int companyId);
        bool Update(PaymentMethod PaymentMethodChanges);
        bool Delete(Guid id);
        bool Create(PaymentMethod NewPaymentMethod);
    }
}
