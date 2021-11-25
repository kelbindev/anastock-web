using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public static class ModelBuilderExtension
    {
        public static void SetRelationship(this ModelBuilder modelBuilder)
        {
            //Common
            modelBuilder.Entity<ProductAndService>()
           .HasOne<Category>(s => s.Category)
           .WithMany(g => g.ProductAndServices)
           .HasForeignKey(s => s.CategoryId);

            modelBuilder.Entity<CustomerAddress>()
             .HasOne<Customer>(s => s.Customer)
             .WithMany(g => g.customerAddresses)
             .HasForeignKey(s => s.CustomerId);

            modelBuilder.Entity<VendorAddress>()
             .HasOne<Vendor>(s => s.Vendor)
             .WithMany(g => g.vendorAddresses)
             .HasForeignKey(s => s.VendorId);

            //Company
            modelBuilder.Entity<ProductAndService>()
     .HasOne<CompanyViewModel>(s => s.Company)
     .WithMany(g => g.ProductAndService)
     .HasForeignKey(s => s.CompanyId)
     .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Customer>()
     .HasOne<CompanyViewModel>(s => s.Company)
     .WithMany(g => g.Customer)
     .HasForeignKey(s => s.CompanyId)
     .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Vendor>()
     .HasOne<CompanyViewModel>(s => s.Company)
     .WithMany(g => g.Vendor)
     .HasForeignKey(s => s.CompanyId)
     .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<ApplicationUser>()
     .HasOne<CompanyViewModel>(s => s.Company)
     .WithMany(g => g.ApplicationUser)
     .HasForeignKey(s => s.CompanyId)
     .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Quote>()
                 .HasOne<CompanyViewModel>(s => s.Company)
                 .WithMany(g => g.Quote)
                 .HasForeignKey(s => s.CompanyId)
                 .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Invoice>()
             .HasOne<CompanyViewModel>(s => s.Company)
             .WithMany(g => g.Invoice)
             .HasForeignKey(s => s.CompanyId)
             .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<PurchaseOrder>()
     .HasOne<CompanyViewModel>(s => s.Company)
     .WithMany(g => g.PurchaseOrder)
     .HasForeignKey(s => s.CompanyId)
     .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Bill>()
    .HasOne<CompanyViewModel>(s => s.Company)
    .WithMany(g => g.Bills)
    .HasForeignKey(s => s.CompanyId)
    .OnDelete(DeleteBehavior.ClientNoAction);

            //Quote
            modelBuilder.Entity<QuoteDetails>()
            .HasOne<Quote>(s => s.Quote)
            .WithMany(g => g.QuoteDetails)
            .HasForeignKey(s => s.QuoteId);

            modelBuilder.Entity<QuoteDetails>()
           .HasOne<ProductAndService>(s => s.ProductService)
           .WithMany(g => g.QuoteDetails)
           .HasForeignKey(s => s.ProductAndServiceId);

            modelBuilder.Entity<Quote>()
                .HasOne<Customer>(s => s.Customer)
                .WithMany(g => g.Quotes)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            //Invoice
            modelBuilder.Entity<InvoiceDetails>()
            .HasOne<Invoice>(s => s.Invoice)
            .WithMany(g => g.invoiceDetails)
            .HasForeignKey(s => s.InvoiceId);

            modelBuilder.Entity<InvoiceDetails>()
           .HasOne<ProductAndService>(s => s.ProductService)
           .WithMany(g => g.InvoiceDetails)
           .HasForeignKey(s => s.ProductAndServiceId);

            modelBuilder.Entity<Invoice>()
               .HasOne<Customer>(s => s.Customer)
               .WithMany(g => g.Invoices)
               .HasForeignKey(s => s.CustomerId)
               .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Invoice>()
           .HasOne<Quote>(s => s.LinkedQuote)
           .WithOne(g => g.Invoice)
           .HasForeignKey<Invoice>(ad => ad.LinkedQuoteId)
           .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<InvoiceReceivable>()
            .HasOne<Invoice>(s => s.LinkedInvoice)
            .WithMany(g => g.invoiceReceivables)
            .HasForeignKey(s => s.LinkedInvoiceId);

            modelBuilder.Entity<InvoiceReceivable>()
           .HasOne<PaymentMethod>(s => s.PaymentMethod)
           .WithMany(g => g.InvoiceReceivables)
           .HasForeignKey(s => s.PaymentMethodId);

            modelBuilder.Entity<Quote>()
           .HasOne<Project>(s => s.Project)
           .WithOne(g => g.Quote)
           .HasForeignKey<Quote>(s => s.LinkedProjectId)
           .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Project>()
            .HasOne<Customer>(s => s.Customer)
            .WithMany(g => g.Projects)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Project>()
            .HasOne<CompanyViewModel>(s => s.Company)
            .WithMany(g => g.Project)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.ClientNoAction);

            //Purchase Order
            modelBuilder.Entity<PurchaseOrderDetails>()
            .HasOne<PurchaseOrder>(s => s.PurchaseOrder)
            .WithMany(g => g.purchaseOrdersDetails)
            .HasForeignKey(s => s.PurchaseOrderId);

            modelBuilder.Entity<PurchaseOrderDetails>()
           .HasOne<ProductAndService>(s => s.ProductService)
           .WithMany(g => g.PurchaseOrderDetails)
           .HasForeignKey(s => s.ProductAndServiceId);

            modelBuilder.Entity<PurchaseOrder>()
          .HasOne<Vendor>(s => s.Vendor)
          .WithMany(g => g.PurchaseOrders)
          .HasForeignKey(s => s.VendorId)
          .OnDelete(DeleteBehavior.ClientNoAction);

            //Bill
            modelBuilder.Entity<BillDetails>()
           .HasOne<Bill>(s => s.Bill)
           .WithMany(g => g.BillDetails)
           .HasForeignKey(s => s.BillId);

            modelBuilder.Entity<BillDetails>()
           .HasOne<ProductAndService>(s => s.ProductService)
           .WithMany(g => g.BillDetails)
           .HasForeignKey(s => s.ProductAndServiceId);

            modelBuilder.Entity<Bill>()
          .HasOne<Vendor>(s => s.Vendor)
          .WithMany(g => g.Bills)
          .HasForeignKey(s => s.VendorId)
          .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Bill>()
          .HasOne<PurchaseOrder>(s => s.LinkedPurchaseOrder)
          .WithMany(g => g.Bill)
          .HasForeignKey(ad => ad.LinkedPOId)
          .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Claim>()
            .HasOne<CompanyViewModel>(s => s.Company)
            .WithMany(g => g.Claim)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Claim>()
            .HasOne<ApplicationUser>(s => s.User)
            .WithMany(g => g.Claim)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<BillPayment>()
           .HasOne<Bill>(s => s.LinkedBill)
           .WithMany(g => g.BillPayments)
           .HasForeignKey(s => s.LinkedBillId);

            modelBuilder.Entity<BillPayment>()
           .HasOne<PaymentMethod>(s => s.PaymentMethod)
           .WithMany(g => g.BillPayments)
           .HasForeignKey(s => s.PaymentMethodId);

            //Stock Balance
            modelBuilder.Entity<ProductBalanceDetails>()
          .HasOne<ProductAndService>(s => s.ProductService)
          .WithMany(g => g.ProductBalanceDetails)
          .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<ProductBalanceDetails>()
        .HasOne<Bill>(s => s.Bill)
        .WithMany(g => g.ProductBalanceDetails)
        .HasForeignKey(s => s.LinkedBillId);

            modelBuilder.Entity<ProductBalanceDetails>()
     .HasOne<Invoice>(s => s.Invoice)
     .WithMany(g => g.ProductBalanceDetails)
     .HasForeignKey(s => s.LinkedInvoiceId);

            modelBuilder.Entity<Activity>()
            .HasOne<CompanyViewModel>(s => s.Company)
            .WithMany(g => g.Activity)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        }
    }
}
