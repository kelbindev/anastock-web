using Anastock.Models;
using Anastock.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IVendorRepository
    {
        Vendor GetVendor(Guid id);
        IEnumerable<Vendor> GetAllVendors(int companyId);
        IEnumerable<Vendor> GetVendorById(int companyId, Guid id);
        Vendor Update(VendorViewModel vendorChanges);
        bool Delete(Guid id);
        VendorAddress GetVendorAddress(Guid VendorId);
        Vendor Create(VendorViewModel NewVendor, int companyId);
    }
}
