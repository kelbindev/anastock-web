using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anastock.Controllers
{
    public class PaymentMethodController : Controller
    {
        private readonly IPaymentMethodRepository _PMRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        int companyId = 0;
        public PaymentMethodController(IPaymentMethodRepository PMRepository, UserManager<ApplicationUser> userManager)
        {
            _PMRepository = PMRepository;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ActiveMenu = "Sales";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                companyId = users.CompanyId;
                var model = _PMRepository.GetAllPaymentMethod(companyId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ModalTitle = "Create Payment Method";
            ViewBag.ButtonText = "Create";
            return PartialView("CreateEdit", new PaymentMethod());
        }
        [HttpGet]
        public PartialViewResult Edit(String pmId)
        {
            ViewBag.ModalTitle = "Edit Product/Service";
            ViewBag.ButtonText = "Update";

            PaymentMethod pmInfo;
            var pm = _PMRepository.GetPaymentMethod(Guid.Parse(pmId));

            if (pm != null)
            {
                pmInfo =
            new PaymentMethod
            {
                PaymentMethodId = pm.PaymentMethodId,
                Description = pm.Description,
                AccountNumber = pm.AccountNumber
            };
                return PartialView("CreateEdit", pmInfo);
            }
            else
            {
                return PartialView("CreateEdit", new PaymentMethod());
            }

        }
        [HttpPost]
        public ActionResult CreateEdit(PaymentMethod NewPM)
        {
            Guid newGuid = NewPM.PaymentMethodId;
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            string msg = "";
            NewPM.CompanyId = companyId;
            if (ModelState.IsValid)
            {
                bool result;
                if (newGuid == Guid.Empty)
                {
                    msg = "Payment Method Created Successfully";
                    result = _PMRepository.Create(NewPM);
                }
                else
                {
                    msg = "Payment Method Updated Successfully";
                    result = _PMRepository.Update(NewPM);
                }


                if (result)
                {
                    return Json(new { success = true, message = msg });
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
                bool result = _PMRepository.Delete(id);

                if (result)
                {
                    return Json(new { success = true, message = "Payment Method Deleted Sucessfully" });
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
