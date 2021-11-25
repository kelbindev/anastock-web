using Anastock.Models;
using Anastock.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface ICustomerRepository 
    {
        Customer GetCustomer(Guid id);
        IEnumerable<Customer> GetAllCustomers(int companyId);
        IEnumerable<Customer> GetCustomerById(int companyId, Guid id);
        Customer Update(CustomerViewModel CustomerChanges);
        bool Delete(Guid id);
        CustomerAddress GetCustomerAddress(Guid CustomerId);
        Customer Create(CustomerViewModel NewCustomer, int companyId);
    }
}
