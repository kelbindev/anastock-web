using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Anastock.Models
{
    public partial class AnastockContext : DbContext
    {
        public AnastockContext()
        {
        }

        public AnastockContext(DbContextOptions<AnastockContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.SetRelationship();

            OnModelCreatingPartial(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
            new Category
            {
                CateogryId = 1,
                CategoryName = "Product"
            },
            new Category
            {
                CateogryId = 2,
                CategoryName = "Service"
            }
            );
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductAndService> ProductAndService { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorAddress> VendorAddresses { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteDetails> QuoteDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetails> billDetails{ get; set; }
        public DbSet<ProductBalance> productBalances { get; set; }
        public DbSet<RoleViewModel> Roles { get; set; }
        public DbSet<CompanyViewModel> Company { get; set; }
        public DbSet<InvoiceReceivable> InvoiceReceivables { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<BillPayment> BillPayments { get; set; }
        public DbSet<ProductBalanceDetails> ProductBalanceDetails { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<ErrorLog> ErrorLog{ get; set; }
    }
}
