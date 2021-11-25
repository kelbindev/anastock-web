using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Anastock.Interfaces
{
    public interface IReportRepository
    {
        #region itemsales
        List<ItemSalesReportViewModel> GetSalesReport(DateTime dateFrom, DateTime dateTo, String status);
        List<ItemSalesChartViewModel> GetItemSalesChart(DateTime fromDate, DateTime toDate);
        #endregion itemsales

        #region sales
        List<SalesDetailsReportViewModel> GetSalesDetails(DateTime dateFrom, DateTime dateTo);
        #endregion sales
    }
}
