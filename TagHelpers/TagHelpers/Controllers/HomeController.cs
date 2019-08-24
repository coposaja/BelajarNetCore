using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TagHelpers.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TagHelpers.Controllers
{
    public class HomeController : Controller
    {
        private IRepository repository;
        public HomeController(IRepository repo)
        {
            repository = repo;
        }

        public IActionResult Index()
        {
            return View(repository.Products);
        }

        public ViewResult Create()
        {
            ViewBag.Quantity = new SelectList(repository.Products.Select(c => c.Quantity).Distinct());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            repository.AddProduct(product);
            return RedirectToAction("Index");
        }

        public ViewResult Edit()
        {
            ViewBag.Quantity = new SelectList(repository.Products.Select(c => c.Quantity).Distinct());
            return View("Create", repository.Products.Last());
        }
    }
}