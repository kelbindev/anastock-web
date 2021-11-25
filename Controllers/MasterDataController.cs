using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Anastock.Controllers
{
    public class MasterDataController : Controller
    {
        private readonly IProductAndServiceRepository itemRepository;
        private readonly IHubContext<SignalServer> context;

        public MasterDataController(IProductAndServiceRepository itemRepository, IHubContext<SignalServer> context)
        {
            this.itemRepository = itemRepository;
            this.context = context;
        }

        public IActionResult ProductAndService()
        {
            return View(itemRepository.All());
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(ProductAndService model)
        {
            if (!ModelState.IsValid) return View(model);

            itemRepository.Add(model);
            itemRepository.SaveChanges();
            context.Clients.All.SendAsync("refreshProductAndService");
            return RedirectToAction("ProductAndService");
        }

        [HttpGet]
        public IActionResult EditProduct(Guid id)
        {
            return View(itemRepository.Find(id));
        }
        [HttpPost]
        public IActionResult EditProduct(ProductAndService model)
        {
            if (!ModelState.IsValid) return View(model);

            var product = itemRepository.Find(model.Id);
            product.Name = model.Name;
            product.Category = model.Category;
            itemRepository.Update(product);
            itemRepository.SaveChanges();

            context.Clients.All.SendAsync("refreshProductAndService");
            return RedirectToAction("ProductAndService");
        }


        [HttpGet]
        public IActionResult DeleteProduct(Guid productId)
        {
            var product = itemRepository.Find(productId);
            itemRepository.Delete(product);
            itemRepository.SaveChanges();
            context.Clients.All.SendAsync("refreshProductAndService");
            return RedirectToAction("ProductAndService");
        }

        [HttpGet]
        public IActionResult GetProductAndServices()
        {
            return Ok(itemRepository.All());
        }
    }
}
