using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        int companyId = 0;
        public VendorController(IVendorRepository vendorRepository, UserManager<ApplicationUser> userManager)
        {
            _vendorRepository = vendorRepository;
            this._userManager = userManager;
        }

        public ViewResult index(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ViewBag.ActiveMenu = "Purchase";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                companyId = users.CompanyId;
                var model = _vendorRepository.GetVendorById(companyId, Guid.Parse(id));
                return View(model);
            }
            else
            {

                ViewBag.ActiveMenu = "Purchase";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                companyId = users.CompanyId;
                var model = _vendorRepository.GetAllVendors(companyId);
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ModalTitle = "Create Vendor";
            ViewBag.ButtonText = "Create";
            return PartialView("CreateEdit", new VendorViewModel());
        }
        [HttpGet]
        public PartialViewResult Edit(String VendorId)
        {
            ViewBag.ModalTitle = "Edit Vendor";
            ViewBag.ButtonText = "Update";

            VendorViewModel vendorInfo;
            var vendor = _vendorRepository.GetVendor(Guid.Parse(VendorId));
            var vendorAddress = _vendorRepository.GetVendorAddress(Guid.Parse(VendorId));

            if (vendor != null)
            {
                vendorInfo =
            new VendorViewModel
            {
                VendorId = vendor.VendorId,
                VendorName = vendor.VendorName,
                VendorEmail = vendor.VendorEmail,
                Description = vendor.Description,
                Website = vendor.Website,
                ShippingAddress = vendorAddress.ShippingAddress,
                ShippingContactEmail = vendorAddress.ShippingContactEmail,
                ShippingContactFax = vendorAddress.ShippingContactFax,
                ShippingContactPerson = vendorAddress.ShippingContactPerson,
                ShippingContactPhone1 = vendorAddress.ShippingContactPhone1,
                ShippingContactPhone2 = vendorAddress.ShippingContactPhone2,
                ShippingContactPhone3 = vendorAddress.ShippingContactPhone3,
                ShippingCountry = vendorAddress.ShippingCountry,
                ShippingPostalCode = vendorAddress.ShippingPostalCode,
                ShippingState = vendorAddress.ShippingState,
                ShippingTown = vendorAddress.ShippingTown,
                BillingAddress = vendorAddress.BillingAddress,
                BillingContactEmail = vendorAddress.BillingContactEmail,
                BillingContactFax = vendorAddress.BillingContactFax,
                BillingContactPerson = vendorAddress.BillingContactPerson,
                BillingContactPhone1 = vendorAddress.BillingContactPhone1,
                BillingContactPhone2 = vendorAddress.BillingContactPhone2,
                BillingContactPhone3 = vendorAddress.BillingContactPhone3,
                BillingCountry = vendorAddress.BillingCountry,
                BillingPostalCode = vendorAddress.BillingPostalCode,
                BillingState = vendorAddress.BillingState,
                BillingTown = vendorAddress.BillingTown
            };
                return PartialView("CreateEdit", vendorInfo);
            }
            else
            {
                return PartialView("CreateEdit", new VendorViewModel());
            }

        }
        [HttpPost]
        public ActionResult CreateEdit(VendorViewModel NewVendor)
        {
            Guid newGuid = NewVendor.VendorId;
            string msg = "";

            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;

            if (ModelState.IsValid)
            {
                Vendor result;
                if (newGuid == Guid.Empty)
                {
                    msg = "Vendor Created Successfully";
                    result = _vendorRepository.Create(NewVendor, companyId);
                }
                else
                {
                    msg = "Vendor Updated Successfully";
                    result = _vendorRepository.Update(NewVendor);
                }

                if (result.VendorId != Guid.Empty)
                {
                    return Json(new
                    {
                        success = true,
                        message = msg,
                        newVendorId = result.VendorId,
                        newVendorName = result.VendorName
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Error Occured, Please try again later" });
                }
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            if (id != Guid.Empty)
            {
                bool result = _vendorRepository.Delete(id);

                if (result)
                {
                    return Json(new { success = true, message = "Vendor Deleted Sucessfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Error Occured, Please try again later" });
                }
            }
            else
            {
                return Json(new { success = false, message = "Error Occured, Please try again later" });
            }
        }
    }
}
