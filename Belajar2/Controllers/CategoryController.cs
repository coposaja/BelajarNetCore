using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Belajar2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Belajar2.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View(new AjaxValidationModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AjaxValidationModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Z", model);
            }

            if (model.Name.Length < 3)
            {
                ModelState.AddModelError("name", "Name should be longer than 2 chars");
                return PartialView("_Z", model);
            }

            ModelState.Clear();
            return PartialView("_Z");
        }
    }
}