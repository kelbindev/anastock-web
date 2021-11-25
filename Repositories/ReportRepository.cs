using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using static Anastock.Code.Common;

namespace Anastock.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        //private Modules module = Modules.;

        public ReportRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }
        #region itemsales
        public List<ItemSalesReportViewModel> GetSalesReport(DateTime dateFrom, DateTime dateTo, string status)
        {
            List<ItemSalesReportViewModel> result =
                (from ps in context.ProductAndService
                 join qd in context.QuoteDetails on ps.Id equals qd.ProductAndServiceId
                 join q in context.Quotes on qd.QuoteId equals q.QuoteId
                 where
                 (q.IssueDate.Date >= dateFrom.Date
                 && q.IssueDate.Date <= dateTo.Date)
                 && q.Status == (status == "0" ? q.Status : status)
                 select new ItemSalesReportViewModel
                 {
                     Name = ps.Name,
                     UOM = qd.UOM,
                     Qty = qd.Qty,
                     Total = qd.Qty * qd.UnitPrice
                 }).ToList();

            List<ItemSalesReportViewModel> grouped =
                (from grp in result
                 group grp by new { grp.Name, grp.UOM } into g1
                 select new ItemSalesReportViewModel
                 {
                     Name = g1.Key.Name,
                     UOM = g1.Key.UOM,
                     Qty = g1.Sum(x => x.Qty),
                     Total = g1.Sum(x => x.Total)
                 }).ToList();

            return grouped;
        }

        #endregion itemsales

        #region sales
        public List<SalesDetailsReportViewModel> GetSalesDetails(DateTime dateFrom, DateTime dateTo)
        {
            List<SalesDetailsReportViewModel> result = new List<SalesDetailsReportViewModel>();

            var totalQuote = (
                from q in context.Quotes
                where q.IsDeleted == false
                && (q.IssueDate.Date >= dateFrom.Date
                && q.IssueDate.Date <= dateTo.Date)
                select q
                );

            decimal totalQuoteValue = (from grp in totalQuote
                                       group grp by 1 into grp1
                                       select grp1.Sum(item => item.Total)).FirstOrDefault();

            result.Add(new SalesDetailsReportViewModel()
            {
                Name = "Total Quote",
                Value = totalQuote.Count(),
                Total = totalQuoteValue
            });

            int totalWonSales = (
                  from q in context.Quotes
                  where q.IsDeleted == false
                  && (q.IssueDate.Date >= dateFrom.Date
                  && q.IssueDate.Date <= dateTo.Date)
                  && (q.Status == "Won")
                  select q
                 ).Count();

            result.Add(new SalesDetailsReportViewModel()
            {
                Name = "Total Won",
                Value = totalWonSales
            });

            int totalLostSales = (
                 from q in context.Quotes
                 where q.IsDeleted == false
                 && (q.IssueDate.Date >= dateFrom.Date
                 && q.IssueDate.Date <= dateTo.Date)
                 && (q.Status == "Lost")
                 select q
                ).Count();

            result.Add(new SalesDetailsReportViewModel()
            {
                Name = "Total Lost",
                Value = totalLostSales
            });

            int totalInvoicedSales = (
                from q in context.Quotes
                where q.IsDeleted == false
                && (q.IssueDate.Date >= dateFrom.Date
                && q.IssueDate.Date <= dateTo.Date)
                && (q.Status == "Invoiced")
                select q
               ).Count();

            result.Add(new SalesDetailsReportViewModel()
            {
                Name = "Total Invoiced",
                Value = totalInvoicedSales
            });

            return result;
        }

        public List<ItemSalesChartViewModel> GetItemSalesChart(DateTime fromDate, DateTime toDate)
        {
            List<ItemSalesChartViewModel> lst = new List<ItemSalesChartViewModel>();

            var items = from q in context.Quotes
                         join qd in context.QuoteDetails on q.QuoteId equals qd.QuoteId
                         join ps in context.ProductAndService on qd.ProductAndServiceId equals ps.Id
                         where q.IssueDate >= fromDate && q.IssueDate <= toDate
                         select new ItemSalesChartViewModel()
                         {
                             ProductName = ps.Name,
                             Total = qd.Total
                         };

            lst = (from grp in items
                   group grp by new { grp.ProductName } into g1
                   select new ItemSalesChartViewModel()
                   {
                       ProductName = g1.Key.ProductName,
                       Total = g1.Sum(x => x.Total)
                   }).ToList();

            return lst;
        }

        #endregion sales
    }
}
