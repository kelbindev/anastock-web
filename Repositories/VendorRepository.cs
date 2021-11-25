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
    public class VendorRepository : IVendorRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.Vendor;

        public VendorRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }

        public IEnumerable<Vendor> GetAllVendors(int companyId)
        {
            return context.Vendors
                .Where(x=>x.IsDeleted == false && x.CompanyId == companyId);
        }
        public IEnumerable<Vendor> GetVendorById(int companyId, Guid id)
        {
            return context.Vendors.Where(c => c.VendorId == id && c.CompanyId == companyId);
        }
        public Vendor GetVendor(Guid id)
        {
            Vendor vendor = context.Vendors.Find(id);
            return vendor;
        }

        public Vendor Update(VendorViewModel VendorInfo)
        {
            Vendor result = new Vendor();
            if (VendorInfo != null)
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                    try
                    {
                        var vendor = context.Vendors.Where(v => v.VendorId == VendorInfo.VendorId).FirstOrDefault();

                        vendor.VendorName = VendorInfo.VendorName;
                        vendor.VendorEmail = VendorInfo.VendorEmail;
                        vendor.Description = VendorInfo.Description;
                        vendor.Website = VendorInfo.Website;
                        vendor.UpdatedBy = userName;
                        vendor.UpdatedDate = DateTime.Now;
                        context.Update(vendor);

                        var vendorAddress = context.VendorAddresses.Where(v => v.VendorId == VendorInfo.VendorId).FirstOrDefault();

                        vendorAddress.ShippingAddress = VendorInfo.ShippingAddress;
                        vendorAddress.ShippingContactEmail = VendorInfo.ShippingContactEmail;
                        vendorAddress.ShippingContactFax = VendorInfo.ShippingContactFax;
                        vendorAddress.ShippingContactPerson = VendorInfo.ShippingContactPerson;
                        vendorAddress.ShippingContactPhone1 = VendorInfo.ShippingContactPhone1;
                        vendorAddress.ShippingContactPhone2 = VendorInfo.ShippingContactPhone2;
                        vendorAddress.ShippingContactPhone3 = VendorInfo.ShippingContactPhone3;
                        vendorAddress.ShippingCountry = VendorInfo.ShippingCountry;
                        vendorAddress.ShippingPostalCode = VendorInfo.ShippingPostalCode;
                        vendorAddress.ShippingState = VendorInfo.ShippingState;
                        vendorAddress.ShippingTown = VendorInfo.ShippingTown;
                        vendorAddress.BillingAddress = VendorInfo.BillingAddress;
                        vendorAddress.BillingContactEmail = VendorInfo.BillingContactEmail;
                        vendorAddress.BillingContactFax = VendorInfo.BillingContactFax;
                        vendorAddress.BillingContactPerson = VendorInfo.BillingContactPerson;
                        vendorAddress.BillingContactPhone1 = VendorInfo.BillingContactPhone1;
                        vendorAddress.BillingContactPhone2 = VendorInfo.BillingContactPhone2;
                        vendorAddress.BillingContactPhone3 = VendorInfo.BillingContactPhone3;
                        vendorAddress.BillingCountry = VendorInfo.BillingCountry;
                        vendorAddress.BillingPostalCode = VendorInfo.BillingPostalCode;
                        vendorAddress.BillingState = VendorInfo.BillingState;
                        vendorAddress.BillingTown = VendorInfo.BillingTown;
                        vendorAddress.UpdatedBy = userName;
                        vendorAddress.UpdatedDate = DateTime.Now;
                        context.Update(vendorAddress);
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = VendorInfo.VendorId,
                            ModuleDescription = VendorInfo.VendorName,
                            ActivityType = ActivityType.Update,
                            User = userName,
                            CompanyId = vendor.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                        result = vendor;
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                        dbContextTransaction.Rollback();
                        loggerRepository.saveError(ex.ToString());
                    }
                }
            }
            return result;
        }

        public VendorAddress GetVendorAddress(Guid VendorId)
        {
            VendorAddress vendorAddress = context.VendorAddresses
                .Where(s => s.VendorId == VendorId).First();
            return vendorAddress;
        }

        public Vendor Create(VendorViewModel VendorInfo, int companyId)
        {
            Vendor result = new Vendor();
            Guid vid = Guid.NewGuid();

            if (VendorInfo != null)
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                    try
                    {
                        Vendor newVendor = new Vendor
                        {
                            VendorId = vid,
                            VendorName = VendorInfo.VendorName,
                            VendorEmail = VendorInfo.VendorEmail,
                            Description = VendorInfo.Description,
                            Website = VendorInfo.Website,
                            CreatedBy = userName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = userName,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false,
                            isActive = true,
                            CompanyId = companyId
                        };
                        context.Vendors.Add(newVendor);

                        VendorAddress newVendorAddress = new VendorAddress
                        {
                            ShippingAddress = VendorInfo.ShippingAddress,
                            ShippingContactEmail = VendorInfo.ShippingContactEmail,
                            ShippingContactFax = VendorInfo.ShippingContactFax,
                            ShippingContactPerson = VendorInfo.ShippingContactPerson,
                            ShippingContactPhone1 = VendorInfo.ShippingContactPhone1,
                            ShippingContactPhone2 = VendorInfo.ShippingContactPhone2,
                            ShippingContactPhone3 = VendorInfo.ShippingContactPhone3,
                            ShippingCountry = VendorInfo.ShippingCountry,
                            ShippingPostalCode = VendorInfo.ShippingPostalCode,
                            ShippingState = VendorInfo.ShippingState,
                            ShippingTown = VendorInfo.ShippingTown,
                            BillingAddress = VendorInfo.BillingAddress,
                            BillingContactEmail = VendorInfo.BillingContactEmail,
                            BillingContactFax = VendorInfo.BillingContactFax,
                            BillingContactPerson = VendorInfo.BillingContactPerson,
                            BillingContactPhone1 = VendorInfo.BillingContactPhone1,
                            BillingContactPhone2 = VendorInfo.BillingContactPhone2,
                            BillingContactPhone3 = VendorInfo.BillingContactPhone3,
                            BillingCountry = VendorInfo.BillingCountry,
                            BillingPostalCode = VendorInfo.BillingPostalCode,
                            BillingState = VendorInfo.BillingState,
                            BillingTown = VendorInfo.BillingTown,
                            CreatedBy = userName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = userName,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false,
                            VendorId = newVendor.VendorId
                        };
                        context.VendorAddresses.Add(newVendorAddress);
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = vid,
                            ModuleDescription = VendorInfo.VendorName,
                            ActivityType = ActivityType.Create,
                            User = userName,
                            CompanyId = companyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                        result = newVendor;
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
                        var vendorAddress = context.VendorAddresses.Where(v => v.VendorId == id).FirstOrDefault();
                        vendorAddress.IsDeleted = true;
                        context.Update(vendorAddress);

                        var vendor = context.Vendors.Where(v => v.VendorId == id).FirstOrDefault();
                        vendor.IsDeleted = true;
                        context.Update(vendor);

                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = id,
                            ModuleDescription = vendor.VendorName,
                            ActivityType = ActivityType.Delete,
                            User = userName,
                            CompanyId = vendor.CompanyId
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
