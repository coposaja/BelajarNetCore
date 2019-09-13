using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belajar10.Models;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Belajar10.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment _env)
        {
            this._env = _env;
        }
        public IActionResult Index()
        {
            List<UserModel> users = new List<UserModel>();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(Path.Combine(_env.WebRootPath, "Report.xlsx"))))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet 1"];
                int totalRows = worksheet.Dimension.Rows;

                for (int i = 2; i <= totalRows; i++)
                {
                    users.Add(new UserModel
                    {
                        Username = worksheet.Cells[i, 1].Value.ToString(),
                        FullName = worksheet.Cells[i, 2].Value.ToString(),
                        Age = Convert.ToInt32(worksheet.Cells[i, 3].Value)
                    });
                }
            }
            return View(users);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
