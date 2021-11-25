using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Models;
using Anastock.ViewModel;
using Anastock.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;

namespace Anastock.Controllers
{
    public class ReportController : Controller
    {
        private readonly AnastockContext context;
        private readonly IReportRepository _reportRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalServer> _signalContext;

        public ReportController(AnastockContext context, IPurchaseOrderRepository poRepository, IReportRepository ReportRepository, UserManager<ApplicationUser> userManager, IHubContext<SignalServer> signalContext)
        {
            this._reportRepository = ReportRepository;
            this._userManager = userManager;
            this.context = context;
            this._signalContext = signalContext;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public JsonResult GetItemSalesReport(DateTime dateFrom, DateTime dateTo, string status)
        {
            var result = _reportRepository.GetSalesReport(dateFrom, dateTo, status);
            //var data = JsonConvert.SerializeObject(new { data = result });
            return Json(result);
        }

        public JsonResult GetSalesDetails(DateTime dateFrom, DateTime DateTo)
        {
            var result = _reportRepository.GetSalesDetails(dateFrom, DateTo);

            return Json(result);
        }

        public List<SalesChartModel> GetSalesData(DateTime dateFrom, DateTime dateTo)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;

            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            //DateTime dtStart = DateTime.Now.AddDays(-(DateTime.Now.Day - 1)).AddMonths(-11);
            //DateTime dtEnd = DateTime.Now;
            DateTime dtStart = dateFrom;
            DateTime dtEnd = dateTo;
            var dts = Enumerable.Range(0, 13).Select(a => dtStart.AddMonths(a))
               .TakeWhile(a => a <= dtEnd)
               .Select(a => String.Concat(a.ToString("MMM") + "'" + a.Year)).AsEnumerable().ToArray();
            List<SalesChartModel> lst = new List<SalesChartModel>();
            for (int i = 0; i < dts.Length; i += 1)
            {
                SalesChartModel scm = new SalesChartModel();
                scm.Period = dts[i];
                scm.Total = 0;
                scm.BalanceDue = 0;
                lst.Add(scm);
            }
            var sales = context.Invoices.Where(
                i => (i.IssueDate >= dtStart && i.IssueDate <= dtEnd) && i.CompanyId == companyId
            ).GroupBy(
                    g => new { Mnth = g.IssueDate.Month, Yr = g.IssueDate.Year.ToString() }
                ).Select(s => new SalesChartModel
                {
                    Period = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(s.Key.Mnth) + "'" + s.Key.Yr,
                    Total = s.Sum(t => t.Total),
                    BalanceDue = s.Sum(b => b.BalanceDue)
                }).ToList();
            foreach (var x in sales)
            {
                var value = lst.FirstOrDefault(d => d.Period == x.Period);
                if (value != null)
                {
                    value.Total = x.Total;
                    value.BalanceDue = x.BalanceDue;
                }
            }
            return lst;
        }

        public List<ItemSalesChartViewModel> GetItemSalesChartData(DateTime dateFrom, DateTime dateTo)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;

            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            //DateTime dtStart = DateTime.Now.AddDays(-(DateTime.Now.Day - 1)).AddMonths(-11);
            //DateTime dtEnd = DateTime.Now;
            DateTime dtStart = dateFrom;
            DateTime dtEnd = dateTo;

            var sales = _reportRepository.GetItemSalesChart(dtStart, dtEnd);
            
            return sales;
        }

    }
}
