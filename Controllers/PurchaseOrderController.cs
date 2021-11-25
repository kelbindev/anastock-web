using Anastock.Interfaces;
using Anastock.Models;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly AnastockContext context;
        private readonly IPurchaseOrderRepository _poRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalServer> _signalContext;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PurchaseOrderController(AnastockContext context, IPurchaseOrderRepository poRepository, UserManager<ApplicationUser> userManager, IHubContext<SignalServer> signalContext, IHostingEnvironment hostingEnvironment)
        {
            this._poRepository = poRepository;
            this._userManager = userManager;
            this.context = context;
            this._signalContext = signalContext;
            this._hostingEnvironment = hostingEnvironment;
            //this.context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.reloadVendor = false;
                ViewBag.ActiveMenu = "Purchase";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                var companyId = users.CompanyId;
                var model = _poRepository.GetPurchaseOrdersByCompanyId(companyId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult New()
        {
            if (User.Identity.IsAuthenticated)
            {
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                ViewBag.ListofVendor = bindVendor();
                //ViewBag.GST = getGST(companyId);
                ViewBag.AutoNumber = AutoNumber(companyId);
                return View();
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
                    PurchaseOrder PurchaseOrderInfo;
                    var PurchaseOrder = _poRepository.GetPurchaseOrder(Guid.Parse(id));
                    var PurchaseOrderDetails = _poRepository.GetPurchaseOrderDetails(Guid.Parse(id));
                    var address = _poRepository.getVendorAddress(PurchaseOrder.VendorAddressId);
                    product = (from p in context.ProductAndService
                               select p).ToList();
                    ViewBag.ProductList = product;
                    if (PurchaseOrder != null)
                    {
                        List<PurchaseOrderDetails> podl = new List<PurchaseOrderDetails>();
                        foreach (var pod in PurchaseOrderDetails)
                        {
                            podl.Add(new PurchaseOrderDetails
                            {
                                PurchaseOrderDetailsId = pod.PurchaseOrderDetailsId,
                                Qty = pod.Qty,
                                UnitPrice = pod.UnitPrice,
                                DiscountPercent = pod.DiscountPercent,
                                Total = pod.Total,
                                PurchaseOrderId = pod.PurchaseOrderId,
                                ProductAndServiceId = pod.ProductAndServiceId,
                                UOM = pod.UOM,
                                Description = pod.Description,
                            });
                        }

                        VendorAddress ca = address;

                        PurchaseOrderInfo = new PurchaseOrder
                        {
                            Id = PurchaseOrder.Id,
                            VendorId = PurchaseOrder.VendorId,
                            PurchaseOrderNo = PurchaseOrder.PurchaseOrderNo,
                            VendorInvoiceNo = PurchaseOrder.VendorInvoiceNo,
                            IssueDate = PurchaseOrder.IssueDate,
                            DueDate = PurchaseOrder.DueDate,
                            Status = PurchaseOrder.Status,
                            VendorNotes = PurchaseOrder.VendorNotes,
                            VendorAddress = ca,
                            VendorAddressId = PurchaseOrder.VendorAddressId,
                            SubTotal = PurchaseOrder.SubTotal,
                            Tax = PurchaseOrder.Tax,
                            Total = PurchaseOrder.Total,
                            CompanyId = companyId,
                            purchaseOrdersDetails = podl,
                            PaymentTerm = PurchaseOrder.PaymentTerm,
                            PaymentTermValue = PurchaseOrder.PaymentTermValue,
                            DiscountType = PurchaseOrder.DiscountType,
                            DiscountValue = PurchaseOrder.DiscountValue
                        };

                        return View(PurchaseOrderInfo);
                    }
                }

                return RedirectToAction("Index", "PurchaseOrder");
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
        public IActionResult save([FromBody] PurchaseOrder po)
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
                PurchaseOrder result = _poRepository.Create(po, companyId); ;
                if (result.Id != Guid.Empty)
                {
                    message = "Purchase Order has been created successfully.";
                    return Json(new { success = true, message = message, poId = result.Id });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message, poId = Guid.Empty });
                }
            }
        }

        [HttpPost]
        public IActionResult update([FromBody] PurchaseOrder po)
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
                bool result = _poRepository.Update(po, companyId); ;
                if (result)
                {
                    message = "Purchase Order has been updated successfully.";
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

            bool result = _poRepository.Delete(id, companyId); ;
            if (result)
            {
                message = "Purchase Order has been deleted successfully.";
                return Json(new { success = true, message = message });
            }
            else
            {
                message = "Error occured. Please try again later.";
                return Json(new { success = false, message = message });
            }
        }

        public IActionResult cancelPO(Guid id)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            string message = "";

            bool result = _poRepository.CancelPO(id, companyId); ;
            if (result)
            {
                message = "Purchase Order has been cancelled successfully.";
                return Json(new { success = true, message = message });
            }
            else
            {
                message = "Error occured. Please try again later.";
                return Json(new { success = false, message = message });
            }
        }

        [HttpGet]
        public ActionResult GeneratePdf(string id)
        {
            RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

            string webRootPath = _hostingEnvironment.WebRootPath;
            Guid poId = new Guid(id);

            int poDetCounts = context.PurchaseOrderDetails.Where(q => q.PurchaseOrderId == poId && q.DiscountPercent > 0).Count();

            string poNo = context.PurchaseOrders.Where(po => po.Id == poId).First().PurchaseOrderNo;
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            string companyName = context.Company.Where(c => c.CompanyId == companyId).FirstOrDefault().Name;

            Config.WebMode = true;
            WebReport webReport = new WebReport();
            if (poDetCounts > 0)
            {
                webReport.Report.Load(webRootPath + "/reports/template/PurchaseOrder.frx");
            }
            else
            {
                webReport.Report.Load(webRootPath + "/reports/template/PurchaseOrderWithoutItemDiscount.frx");
            }
            webReport.Report.Dictionary.Connections[0].ConnectionString = context.Database.GetDbConnection().ConnectionString;
            webReport.Report.StartReport += delegate (object sender, EventArgs e)
            {
                webReport.Report.SetParameterValue("poId", poId);
                webReport.Report.SetParameterValue("companyName", companyName);
            };

            webReport.Report.Prepare();
            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                pdfExport.Export(webReport.Report, ms);
                ms.Flush();
                return File(ms.ToArray(), "application/pdf", Path.GetFileNameWithoutExtension("PurchaseOrder") +
                    String.Format("_{0}.pdf", poNo));
            }

        }

        private String AutoNumber(int companyId)
        {
            string max = context.PurchaseOrders.Where(po => po.PurchaseOrderNo.StartsWith("PO-") && po.PurchaseOrderNo.Length == 10 && po.CompanyId == companyId).Select(po => po.PurchaseOrderNo).Max();
            string anumber = "0";
            if (String.IsNullOrEmpty(max))
            {
                anumber = "PO-" + 1.ToString().PadLeft(7, '0');
            }
            else
            {
                int lastNumber = Int32.Parse(max.Split("-").Last());
                anumber = "PO-" + (lastNumber + 1).ToString().PadLeft(7, '0');
            }
            return anumber;
        }
    }
}
