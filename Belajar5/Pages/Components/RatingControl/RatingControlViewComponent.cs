using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belajar5.Pages.Components.RatingControl
{
    public class RatingControlViewComponent : ViewComponent
    {
        public RatingControlViewComponent()
        {

        }

        public IViewComponentResult Invoke(string ratingControlType)
        {
            System.Threading.Thread.Sleep(5000);
            return View("Default", ratingControlType);
        }
    }
}
