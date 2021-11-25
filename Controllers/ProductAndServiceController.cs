using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anastock.Controllers
{
    public class ProductAndServiceController : Controller
    {
        private readonly IProductAndServiceRepository _PSRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerRepository loggerRepository;
        int companyId = 0;
        public ProductAndServiceController(IProductAndServiceRepository PSRepository, UserManager<ApplicationUser> userManager, ILoggerRepository loggerRepository)
        {
            _PSRepository = PSRepository;
            this._userManager = userManager;
            this.loggerRepository = loggerRepository;
        }
        public IActionResult Index(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    companyId = users.CompanyId;
                    var model = _PSRepository.GetProductAndServiceById(companyId, Guid.Parse(id));
                    return View(model);
                }
                else
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    companyId = users.CompanyId;
                    var model = _PSRepository.GetAllProductAndService(companyId);
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
            ViewBag.ModalTitle = "Create Product/Service";
            ViewBag.ButtonText = "Create";
            return PartialView("CreateEdit", new ProductAndServiceViewModel());
        }
        [HttpGet]
        public PartialViewResult Edit(String psId)
        {
            ViewBag.ModalTitle = "Edit Product/Service";
            ViewBag.ButtonText = "Update";

            ProductAndServiceViewModel psInfo;
            var ps = _PSRepository.GetProductAndService(Guid.Parse(psId));

            if (ps != null)
            {
                psInfo =
            new ProductAndServiceViewModel
            {
                ProductAndServiceId = ps.Id,
                Name = ps.Name,
                CategoryId = ps.CategoryId,
                isSell = ps.isSell,
                SellPrice = ps.SellPrice,
                SellUOM = ps.SellUOM,
                isPurchase = ps.isPurchase,
                PurchasePrice = ps.PurchasePrice,
                PurchaseUOM = ps.PurchaseUOM,
                isActive = ps.isActive,
                SellQty = ps.SellQty,
                PurchaseQty= ps.PurchaseQty,
                UOM = ps.UOM
            };
                return PartialView("CreateEdit", psInfo);
            }
            else
            {
                return PartialView("CreateEdit", new ProductAndServiceViewModel());
            }

        }
        [HttpPost]
        public ActionResult CreateEdit(ProductAndServiceViewModel NewPS)
        {
            try
            {
                Guid newGuid = NewPS.ProductAndServiceId;
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                string msg = "";
                NewPS.SellPrice = NewPS.SellPrice == null ? 0 : NewPS.SellPrice;
                NewPS.PurchasePrice = NewPS.PurchasePrice == null ? 0 : NewPS.PurchasePrice;
                NewPS.CompanyId = companyId;
                if (ModelState.IsValid)
                {
                    bool result;
                    if (newGuid == Guid.Empty)
                    {
                        msg = "Product/Service Created Successfully";
                        result = _PSRepository.Create(NewPS);
                    }
                    else
                    {
                        msg = "Product/Service Updated Successfully";
                        result = _PSRepository.Update(NewPS);
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
                    loggerRepository.saveError("Empty Model");
                    return View();
                }
            }
            catch (Exception ex)
            {
                loggerRepository.saveError(ex.ToString());
                return View();
            }
           
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            if (id != Guid.Empty)
            {
                bool result = _PSRepository.Delete(id);

                if (result)
                {
                    return Json(new { success = true, message = "Product/Service Deleted Sucessfully" });
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
        [HttpGet]
        public IActionResult ProductBalance ()
        {
            if (User.Identity.IsAuthenticated)
            {
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;

                IEnumerable<ProductBalanceViewModel> stockBalance = _PSRepository.getStockBalance(companyId);
                return View(stockBalance);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public IActionResult ProductBalanceDetails(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ProductAndService p = _PSRepository.GetProductAndService(id);

                ViewBag.ProductName = p.Name;

                IEnumerable<ProductBalanceDetailsViewModel> stockBalanceDetails = _PSRepository.getStockBalanceDetails(id);
                return View(stockBalanceDetails);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
