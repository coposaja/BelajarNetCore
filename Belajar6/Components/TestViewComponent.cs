using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belajar6.Components
{
    public class TestViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string type)
        {
            if (type == "test")
            {
                return Test();
            }
            return BukanTest();
        }

        public IViewComponentResult Test()
        {
            return View("~/Components/Test.cshtml");
        }
        public IViewComponentResult BukanTest()
        {
            return View("~/Components/BukanTest.cshtml");
        }
    }
}
