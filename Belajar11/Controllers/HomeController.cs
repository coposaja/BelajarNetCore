using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belajar11.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DinkToPdf.Contracts;
using DinkToPdf;

namespace Belajar11.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IConverter _converter;
        public HomeController(IHostingEnvironment _env, IConverter _converter)
        {
            this._env = _env;
            this._converter = _converter;
        }

        public IActionResult Index()
        {
            string content;
            using (StreamReader sr = new StreamReader(Path.Combine(_env.WebRootPath, "template.html")))
            {
                List<UserModel> users = new List<UserModel>()
                {
                    new UserModel() { Username = "christyantofernando", FullName = "Fernando Christyanto", Age = 21 },
                    new UserModel() { Username = "coposaja", FullName = "Yohanzen Christanto Alexander", Age = 21 },
                    new UserModel() { Username = "tmusthofa", FullName = "Tegar Aryo Sulthon Musthofa", Age = 32 },
                    new UserModel() { Username = "nubs", FullName = "Nubli Hawari", Age = 28 },
                    new UserModel() { Username = "nadiaita", FullName = "Nadia Ita Purnamasari", Age = 20 }
                };
                content = sr.ReadToEnd();
                content = content.Replace("<%-- Jumlah User --%>", users.Count.ToString());
                content = content.Replace("<%-- User Table --%>", GetUserTable(users));
            }

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "PDF Test"
                },
                Objects = { new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = content,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontName = "Times New Roman", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Times New Roman", FontSize = 9, Line = true, Center = "Report Footer" }
                } }
            };

            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }

        private string GetUserTable(List<UserModel> users)
        {
            StringBuilder sb = new StringBuilder();
            foreach (UserModel user in users)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", user.Username, user.FullName, user.Age);
            }
            return sb.ToString();
        }
    }
}
