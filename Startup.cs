using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Anastock.Models;
using Microsoft.AspNetCore.Identity;
using Anastock.Interfaces;
using Anastock.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Anastock
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("AnastockDatabase");
            services.AddTransient<IVendorRepository, VendorRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IQuoteRepository, QuoteRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddTransient<IBillRepository, BillRepository>();
            services.AddTransient<IProjectRepository, ProjectRepository>();
            services.AddTransient<IInvoiceReceivableRepository, InvoiceReceivableRepository>();
            services.AddTransient<IProductAndServiceRepository, ProductAndServiceRepository>();
            services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ILoggerRepository, LoggerRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();
            //services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<AnastockContext>(options => options.UseSqlServer(connection));
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            services.AddHttpContextAccessor();
            services.AddSignalR();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.ConfigureExceptionHandler(logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseFastReport();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "areas", "areas", 
                    pattern: "{area:exists}/{controller=Registration}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

                endpoints.MapHub<SignalServer>("/signalServer");
            });
        }
    }
}
