using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _CustomerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        int companyId = 0;
        public CustomerController(ICustomerRepository CustomerRepository, UserManager<ApplicationUser> userManager)
        {
            _CustomerRepository = CustomerRepository;
            this._userManager = userManager;
        }

        public IActionResult index(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    ViewBag.ActiveMenu = "Purchase";
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    companyId = users.CompanyId;
                    var model = _CustomerRepository.GetCustomerById(companyId, Guid.Parse(id));
                    return View(model);
                }
                else
                {
                    ViewBag.ActiveMenu = "Purchase";
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    companyId = users.CompanyId;
                    var model = _CustomerRepository.GetAllCustomers(companyId);
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ModalTitle = "Create Customer";
            ViewBag.ButtonText = "Create";
            return PartialView("CreateEdit", new CustomerViewModel());
        }
        [HttpGet]
        public PartialViewResult Edit(String CustomerId)
        {
            ViewBag.ModalTitle = "Edit Customer";
            ViewBag.ButtonText = "Update";

            CustomerViewModel CustomerInfo;
            var Customer = _CustomerRepository.GetCustomer(Guid.Parse(CustomerId));
            var CustomerAddress = _CustomerRepository.GetCustomerAddress(Guid.Parse(CustomerId));

            if (Customer != null)
            {
                CustomerInfo =
            new CustomerViewModel
            {
                CustomerId = Customer.CustomerId,
                CustomerName = Customer.CustomerName,
                CustomerEmail = Customer.CustomerEmail,
                Description = Customer.Description,
                Website = Customer.Website,
                ShippingAddress = CustomerAddress.ShippingAddress,
                ShippingContactEmail = CustomerAddress.ShippingContactEmail,
                ShippingContactFax = CustomerAddress.ShippingContactFax,
                ShippingContactPerson = CustomerAddress.ShippingContactPerson,
                ShippingContactPhone1 = CustomerAddress.ShippingContactPhone1,
                ShippingContactPhone2 = CustomerAddress.ShippingContactPhone2,
                ShippingContactPhone3 = CustomerAddress.ShippingContactPhone3,
                ShippingCountry = CustomerAddress.ShippingCountry,
                ShippingPostalCode = CustomerAddress.ShippingPostalCode,
                ShippingState = CustomerAddress.ShippingState,
                ShippingTown = CustomerAddress.ShippingTown,
                BillingAddress = CustomerAddress.BillingAddress,
                BillingContactEmail = CustomerAddress.BillingContactEmail,
                BillingContactFax = CustomerAddress.BillingContactFax,
                BillingContactPerson = CustomerAddress.BillingContactPerson,
                BillingContactPhone1 = CustomerAddress.BillingContactPhone1,
                BillingContactPhone2 = CustomerAddress.BillingContactPhone2,
                BillingContactPhone3 = CustomerAddress.BillingContactPhone3,
                BillingCountry = CustomerAddress.BillingCountry,
                BillingPostalCode = CustomerAddress.BillingPostalCode,
                BillingState = CustomerAddress.BillingState,
                BillingTown = CustomerAddress.BillingTown
            };
                return PartialView("CreateEdit", CustomerInfo);
            }
            else
            {
                return PartialView("CreateEdit", new CustomerViewModel());
            }

        }
        [HttpPost]
        public ActionResult CreateEdit(CustomerViewModel NewCustomer)
        {
            Guid newGuid = NewCustomer.CustomerId;
            string msg = "";

            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;

            if (ModelState.IsValid)
            {
                Customer result;
                if (newGuid == Guid.Empty)
                {
                    msg = "Customer Created Successfully";
                    result = _CustomerRepository.Create(NewCustomer, companyId);
                }
                else
                {
                    msg = "Customer Updated Successfully";
                    result = _CustomerRepository.Update(NewCustomer);
                }

                if (result.CustomerId != Guid.Empty)
                {
                    return Json(new { success = true, message = msg,
                        newCustomerId = result.CustomerId,
                        newCustomerName = result.CustomerName
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
                bool result = _CustomerRepository.Delete(id);

                if (result)
                {
                    return Json(new { success = true, message = "Customer Deleted Sucessfully" });
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
