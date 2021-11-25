using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Anastock.Code.Common;

namespace Anastock.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.Customer;

        public CustomerRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }

        public IEnumerable<Customer> GetAllCustomers(int companyId)
        {
            return context.Customers
                .Where(x => x.IsDeleted == false && x.CompanyId == companyId);
        }

        public IEnumerable<Customer> GetCustomerById(int companyId, Guid id)
        {
            return context.Customers.Where(c => c.CustomerId == id && c.CompanyId == companyId);
        }

        public Customer GetCustomer(Guid id)
        {
            Customer Customer = context.Customers.Find(id);
            return Customer;
        }

        public Customer Update(CustomerViewModel CustomerInfo)
        {
            Customer result = new Customer();
            if (CustomerInfo != null)
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                    try
                    {
                        var Customer = context.Customers.Where(v => v.CustomerId == CustomerInfo.CustomerId).FirstOrDefault();

                        Customer.CustomerName = CustomerInfo.CustomerName;
                        Customer.CustomerEmail = CustomerInfo.CustomerEmail;
                        Customer.Description = CustomerInfo.Description;
                        Customer.Website = CustomerInfo.Website;
                        Customer.UpdatedBy = userName;
                        Customer.UpdatedDate = DateTime.Now;
                        context.Update(Customer);

                        var CustomerAddress = context.CustomerAddresses.Where(v => v.CustomerId == CustomerInfo.CustomerId).FirstOrDefault();

                        CustomerAddress.ShippingAddress = CustomerInfo.ShippingAddress;
                        CustomerAddress.ShippingContactEmail = CustomerInfo.ShippingContactEmail;
                        CustomerAddress.ShippingContactFax = CustomerInfo.ShippingContactFax;
                        CustomerAddress.ShippingContactPerson = CustomerInfo.ShippingContactPerson;
                        CustomerAddress.ShippingContactPhone1 = CustomerInfo.ShippingContactPhone1;
                        CustomerAddress.ShippingContactPhone2 = CustomerInfo.ShippingContactPhone2;
                        CustomerAddress.ShippingContactPhone3 = CustomerInfo.ShippingContactPhone3;
                        CustomerAddress.ShippingCountry = CustomerInfo.ShippingCountry;
                        CustomerAddress.ShippingPostalCode = CustomerInfo.ShippingPostalCode;
                        CustomerAddress.ShippingState = CustomerInfo.ShippingState;
                        CustomerAddress.ShippingTown = CustomerInfo.ShippingTown;
                        CustomerAddress.BillingAddress = CustomerInfo.BillingAddress;
                        CustomerAddress.BillingContactEmail = CustomerInfo.BillingContactEmail;
                        CustomerAddress.BillingContactFax = CustomerInfo.BillingContactFax;
                        CustomerAddress.BillingContactPerson = CustomerInfo.BillingContactPerson;
                        CustomerAddress.BillingContactPhone1 = CustomerInfo.BillingContactPhone1;
                        CustomerAddress.BillingContactPhone2 = CustomerInfo.BillingContactPhone2;
                        CustomerAddress.BillingContactPhone3 = CustomerInfo.BillingContactPhone3;
                        CustomerAddress.BillingCountry = CustomerInfo.BillingCountry;
                        CustomerAddress.BillingPostalCode = CustomerInfo.BillingPostalCode;
                        CustomerAddress.BillingState = CustomerInfo.BillingState;
                        CustomerAddress.BillingTown = CustomerInfo.BillingTown;
                        CustomerAddress.UpdatedBy = userName;
                        CustomerAddress.UpdatedDate = DateTime.Now;
                        context.Update(CustomerAddress);
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = CustomerInfo.CustomerId,
                            ModuleDescription = CustomerInfo.CustomerName,
                            ActivityType = ActivityType.Update,
                            User = userName,
                            CompanyId = Customer.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                        result = Customer;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        loggerRepository.saveError(ex.ToString());
                    }
                }
            }
            return result;
        }

        public CustomerAddress GetCustomerAddress(Guid CustomerId)
        {
            CustomerAddress CustomerAddress = context.CustomerAddresses
                .Where(s => s.CustomerId == CustomerId).First();
            return CustomerAddress;
        }

        public Customer Create(CustomerViewModel CustomerInfo, int companyId)
        {
            Customer result = new Customer();
            Guid cid = Guid.NewGuid();

            if (CustomerInfo != null)
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                    try
                    {
                        Customer newCustomer = new Customer
                        {
                            CustomerId = cid,
                            CustomerName = CustomerInfo.CustomerName,
                            CustomerEmail = CustomerInfo.CustomerEmail,
                            Description = CustomerInfo.Description,
                            Website = CustomerInfo.Website,
                            CreatedBy = userName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = userName,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false,
                            isActive = true,
                            CompanyId = companyId
                        };
                        context.Customers.Add(newCustomer);

                        CustomerAddress newCustomerAddress = new CustomerAddress
                        {
                            ShippingAddress = CustomerInfo.ShippingAddress,
                            ShippingContactEmail = CustomerInfo.ShippingContactEmail,
                            ShippingContactFax = CustomerInfo.ShippingContactFax,
                            ShippingContactPerson = CustomerInfo.ShippingContactPerson,
                            ShippingContactPhone1 = CustomerInfo.ShippingContactPhone1,
                            ShippingContactPhone2 = CustomerInfo.ShippingContactPhone2,
                            ShippingContactPhone3 = CustomerInfo.ShippingContactPhone3,
                            ShippingCountry = CustomerInfo.ShippingCountry,
                            ShippingPostalCode = CustomerInfo.ShippingPostalCode,
                            ShippingState = CustomerInfo.ShippingState,
                            ShippingTown = CustomerInfo.ShippingTown,
                            BillingAddress = CustomerInfo.BillingAddress,
                            BillingContactEmail = CustomerInfo.BillingContactEmail,
                            BillingContactFax = CustomerInfo.BillingContactFax,
                            BillingContactPerson = CustomerInfo.BillingContactPerson,
                            BillingContactPhone1 = CustomerInfo.BillingContactPhone1,
                            BillingContactPhone2 = CustomerInfo.BillingContactPhone2,
                            BillingContactPhone3 = CustomerInfo.BillingContactPhone3,
                            BillingCountry = CustomerInfo.BillingCountry,
                            BillingPostalCode = CustomerInfo.BillingPostalCode,
                            BillingState = CustomerInfo.BillingState,
                            BillingTown = CustomerInfo.BillingTown,
                            CreatedBy = userName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = userName,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false,
                            CustomerId = newCustomer.CustomerId
                        };
                        context.CustomerAddresses.Add(newCustomerAddress);
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = cid,
                            ModuleDescription = CustomerInfo.CustomerName,
                            ActivityType = ActivityType.Create,
                            User = userName,
                            CompanyId = companyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                        result = newCustomer;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        loggerRepository.saveError(ex.ToString());
                    }
                }
            }
            return result;
        }

        public bool Delete(Guid id)
        {
            bool result = true;
            if (id != Guid.Empty)
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                    try
                    {
                        var CustomerAddress = context.CustomerAddresses.Where(v => v.CustomerId == id).FirstOrDefault();
                        CustomerAddress.IsDeleted = true;
                        context.Update(CustomerAddress);

                        var Customer = context.Customers.Where(v => v.CustomerId == id).FirstOrDefault();
                        Customer.IsDeleted = true;
                        context.Update(Customer);

                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = id,
                            ModuleDescription = Customer.CustomerName,
                            ActivityType = ActivityType.Delete,
                            User = userName,
                            CompanyId = Customer.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        result = false;
                        loggerRepository.saveError(ex.ToString());
                    }
                }
            }
            return result;
        }
    }
}
