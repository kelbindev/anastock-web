using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Controllers
{
    public class BillController : Controller
    {
        private readonly AnastockContext context;
        private readonly IBillRepository _billRepository;
        private readonly IPurchaseOrderRepository _poRepository ;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalServer> _signalContext;

        public BillController(AnastockContext context,IPurchaseOrderRepository poRepository, IBillRepository billRepository, UserManager<ApplicationUser> userManager, IHubContext<SignalServer> signalContext)
        {
            this._billRepository = billRepository;
            this._poRepository = poRepository;
            this._userManager = userManager;
            this.context = context;
            this._signalContext = signalContext;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ActiveMenu = "Purchase";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                var companyId = users.CompanyId;
                var model = _billRepository.GetBillsWithPOByCompanyId(companyId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult New(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    ViewBag.ListofVendor = bindVendor();
                    ViewBag.NewId = id;
                    ViewBag.AutoNumber = AutoNumber(companyId);
                    return View(new Bill());
                }
                else
                {
                    List<ProductAndService> product = new List<ProductAndService>();
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    string projectTitle = "-";
                    ViewBag.ListofVendor = bindVendor();
                    ViewBag.NewId = id;
                    string autoNumber = AutoNumber(companyId);
                    ViewBag.AutoNumber = autoNumber;
                    product = (from p in context.ProductAndService
                               select p).ToList();
                    ViewBag.ProductList = product;
                    Bill billInfo;
                    var po = _poRepository.GetPurchaseOrder(id);
                    var poDetails = _poRepository.GetPurchaseOrderDetails(id);
                   
                    ViewBag.ProjectTitle = projectTitle;
                    product = (from p in context.ProductAndService
                               select p).ToList();
                    ViewBag.ProductList = product;
                    if (po != null)
                    {
                        var address = _billRepository.getVendorAddress(po.VendorAddressId);
                        List<BillDetails> bd = new List<BillDetails>();
                        foreach (var pod in poDetails)
                        {
                            bd.Add(new BillDetails
                            {
                                Qty = pod.Qty,
                                UnitPrice = pod.UnitPrice,
                                DiscountPercent = pod.DiscountPercent,
                                Total = pod.Total,
                                ProductAndServiceId = pod.ProductAndServiceId,
                                Description = pod.Description,
                                UOM = pod.UOM,
                            });
                        }

                        VendorAddress va = address;

                        billInfo = new Bill
                        {
                            LinkedPOId = po.Id,
                            VendorId = po.VendorId,
                            BillNo = autoNumber,
                            VendorInvoiceNo = po.VendorInvoiceNo,
                            IssueDate = po.IssueDate,
                            DueDate = po.DueDate,
                            Status = po.Status,
                            VendorNotes = po.VendorNotes,
                            VendorAddress = va,
                            VendorAddressId = po.VendorAddressId,
                            SubTotal = po.SubTotal,
                            Tax = po.Tax,
                            Total = po.Total,
                            PaymentTerm = po.PaymentTerm,
                            PaymentTermValue = po.PaymentTermValue,
                            CompanyId = companyId,
                            DiscountType = po.DiscountType,
                            DiscountValue = po.DiscountValue,
                            BillDetails = bd
                        };

                        return View(billInfo);
                    }
                    return RedirectToAction("Index", "Bill");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    List<ProductAndService> product = new List<ProductAndService>();
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    ViewBag.ListofVendor = bindVendor();
                    //ViewBag.GST = getGST(companyId);
                    Bill BillInfo;
                    var Bill = _billRepository.GetBill(Guid.Parse(id));
                    var BillDetails = _billRepository.GetBillDetails(Guid.Parse(id));
                    var address = _billRepository.getVendorAddress(Bill.VendorAddressId);
                    product = (from p in context.ProductAndService
                               select p).ToList();
                    ViewBag.ProductList = product;
                    if (Bill != null)
                    {
                        List<BillDetails> billdl = new List<BillDetails>();
                        foreach (var billd in BillDetails)
                        {
                            billdl.Add(new BillDetails
                            {
                                BillDetailsId = billd.BillDetailsId,
                                Qty = billd.Qty,
                                UnitPrice = billd.UnitPrice,
                                DiscountPercent = billd.DiscountPercent,
                                Total = billd.Total,
                                BillId = billd.BillId,
                                ProductAndServiceId = billd.ProductAndServiceId,
                                UOM = billd.UOM,
                                Description = billd.Description,
                            });
                        }

                        VendorAddress ca = address;

                        BillInfo = new Bill
                        {
                            Id = Bill.Id,
                            VendorId = Bill.VendorId,
                            BillNo = Bill.BillNo,
                            VendorInvoiceNo = Bill.VendorInvoiceNo,
                            IssueDate = Bill.IssueDate,
                            DueDate = Bill.DueDate,
                            Status = Bill.Status,
                            VendorNotes = Bill.VendorNotes,
                            VendorAddress = ca,
                            VendorAddressId = Bill.VendorAddressId,
                            SubTotal = Bill.SubTotal,
                            Tax = Bill.Tax,
                            Total = Bill.Total,
                            CompanyId = companyId,
                            BillDetails = billdl,
                            PaymentTerm = Bill.PaymentTerm,
                            PaymentTermValue = Bill.PaymentTermValue,
                            DiscountType = Bill.DiscountType,
                            DiscountValue = Bill.DiscountValue
                        };

                        return View(BillInfo);
                    }
                }

                return RedirectToAction("Index", "Bill");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [ValidateAntiForgeryToken]
        private List<Vendor> bindVendor()
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            var companyId = users.CompanyId;

            List<Vendor> vendorList = new List<Vendor>();
            vendorList = (from v in context.Vendors
                          where v.CompanyId == companyId
                          && v.IsDeleted == false
                          && v.isActive == true
                          select v).ToList();
            vendorList.Insert(0, new Vendor { VendorId = new Guid("00000000-0000-0000-0000-000000000000"), VendorName = "Select Vendor" });
            vendorList.Insert(vendorList.Count, new Vendor { VendorId = new Guid("00000000-0000-0000-0000-000000000000"), VendorName = "Create New Vendor" });
            return vendorList;
        }

        public List<VendorAddress> bindvendorAddressById(Guid vendorId)
        {
            List<VendorAddress> VendorAddressList = new List<VendorAddress>();
            VendorAddressList = (from va in context.VendorAddresses
                                 where va.VendorId == vendorId
                                 select va).ToList();
            return VendorAddressList;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            var companyId = users.CompanyId;

            List<ProductAndService> productList = new List<ProductAndService>();
            productList = (from product in context.ProductAndService
                           where product.CompanyId == companyId
                           select product).ToList();

            return Ok(productList);
        }

        [HttpPost]
        public IActionResult save([FromBody] Bill bill)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<ModelErrorCollection> error = null;
            string message = "";

            if (!ModelState.IsValid)
            {
                error = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Json(new { success = false, message = String.Join(Environment.NewLine, error) });
            }
            else
            {
                Bill result = _billRepository.Create(bill, companyId); ;
                if (result.Id != Guid.Empty)
                {
                    message = "Bill has been created successfully.";
                    return Json(new { success = true, message = message, billId = result.Id });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message, billId = Guid.Empty });
                }
            }
        }

        [HttpPost]
        public IActionResult update([FromBody] Bill bill)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<ModelErrorCollection> error = null;
            string message = "";

            if (!ModelState.IsValid)
            {
                error = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Json(new { success = false, message = String.Join(Environment.NewLine, error) });
            }
            else
            {
                bool result = _billRepository.Update(bill, companyId); ;
                if (result)
                {
                    message = "Bill has been updated successfully.";
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
            }
        }

        public IActionResult delete(Guid id)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            string message = "";

            bool result = _billRepository.Delete(id, companyId); ;
            if (result)
            {
                message = "Bill has been deleted successfully.";
                return Json(new { success = true, message = message });
            }
            else
            {
                message = "Error occured. Please try again later.";
                return Json(new { success = false, message = message });
            }
        }

        //Bill Payment
        public IActionResult IndexPayment()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ActiveMenu = "Purchase";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                var model = _billRepository.GetBillPayments(companyId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult savePayment([FromBody] BillPaymentViewModel billPayment)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<ModelErrorCollection> error = null;
            string message = "";

            if (!ModelState.IsValid)
            {
                error = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Json(new { success = false, message = String.Join(Environment.NewLine, error) });
            }
            else
            {
                bool result = _billRepository.CreatePayment(billPayment, companyId); ;
                if (result)
                {
                    message = "Bill Payment has been created successfully.";
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
            }
        }

        public IActionResult NewPayment(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    ViewBag.ListofVendor = bindVendor();
                    ViewBag.ListofPayment = bindPayment(companyId);
                    ViewBag.NewId = id;
                    return View();
                }
                else
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    ViewBag.ListofPayment = bindPayment(companyId);
                    ViewBag.NewId = id;
                    var Bill = _billRepository.GetBill(id);
                    if (Bill != null)
                    {
                        ViewBag.ListofVendor = new SelectList(bindVendor(), "VendorId", "VendorName", Bill.VendorId);
                        ViewBag.ListofBill = new SelectList(bindBills(Bill.VendorId, companyId), "Id", "BillNo", Bill.Id);
                        ViewBag.BalanceDue = Bill.BalanceDue.ToString("0.00");
                        return View();
                    }
                    return RedirectToAction("IndexPayment", "Bill");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        private List<PaymentMethod> bindPayment(int companyId)
        {
            List<PaymentMethod> payMethod = new List<PaymentMethod>();
            payMethod = (from c in context.PaymentMethods
                         where c.CompanyId == companyId && c.IsDeleted == false
                         select c).ToList();
            payMethod.Insert(0, new PaymentMethod { PaymentMethodId = new Guid("00000000-0000-0000-0000-000000000000"), Description = "Select Payment" });
            return payMethod;
        }

        public List<Bill> bindBills(Guid vendorId, int companyId)
        {
            List<Bill> Bill = new List<Bill>();
            Bill = (from c in context.Bills
                       where c.CompanyId == companyId && c.VendorId == vendorId && c.IsDeleted == false && (c.Status == "Open" || c.Status == "On Progress")
                       select c).ToList();
            Bill.Insert(0, new Bill { Id = new Guid("00000000-0000-0000-0000-000000000000"), BillNo = "Select Bill" });
            return Bill;
        }

        public ActionResult bindBill(Guid vendorId)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<Bill> Bill = new List<Bill>();
            Bill = (from c in context.Bills
                       where c.CompanyId == companyId && c.VendorId == vendorId && c.IsDeleted == false && (c.Status == "Open" || c.Status == "On Progress")
                       select c).ToList();
            Bill.Insert(0, new Bill { Id = new Guid("00000000-0000-0000-0000-000000000000"), BillNo = "Select Bill" });
            return Ok(Bill);
        }

        public ActionResult bindBalance(Guid BillId)
        {
            var balance = context.Bills.Where(i => i.Id == BillId).Select(i => i.BalanceDue).Single();
            return Ok(balance);
        }

        private String AutoNumber(int companyId)
        {
            string max = context.Bills.Where(b => b.BillNo.StartsWith("B-") && b.BillNo.Length == 9 && b.CompanyId == companyId).Select(b => b.BillNo).Max();
            string anumber = "0";
            if (String.IsNullOrEmpty(max))
            {
                anumber = "B-" + 1.ToString().PadLeft(7, '0');
            }
            else
            {
                int lastNumber = Int32.Parse(max.Split("-").Last());
                anumber = "B-" + (lastNumber + 1).ToString().PadLeft(7, '0');
            }
            return anumber;
        }
    }
}
