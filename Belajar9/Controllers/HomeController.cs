using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belajar9.Models;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Data;
using System.Net.Mime;

namespace Belajar9.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet = GetWorksheet(worksheet);

                string path = Path.Combine(hostingEnvironment.WebRootPath, DateTime.Now.ToFileTime().ToString(), ".xlsx");
                FileInfo file = new FileInfo("D:/Belajar/Hansen.xlsx");
                package.SaveAs(file);
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes("D:/Belajar/Hansen.xlsx");
            FileInfo deleteFile = new FileInfo("D:/Belajar/Hansen.xlsx");
            deleteFile.Delete();
            return File(fileBytes, MediaTypeNames.Application.Octet, "Report.xlsx");
        }

        private ExcelWorksheet GetWorksheet(ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1"].LoadFromDataTable(GetData(), true);
            worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.Fill.BackgroundColor.SetColor(1, 79, 129, 189);
            worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.White);
            worksheet.Tables.Add(worksheet.Cells[worksheet.Dimension.Address], "Table1");
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return worksheet;
        }

        private DataTable GetData()
        {
            List<UserModel> userDatas = new List<UserModel>()
            {
                new UserModel() { Username = "christyantofernando", FullName = "Fernando Christyanto", Age = 21 },
                new UserModel() { Username = "coposaja", FullName = "Yohanzen Christanto Alexander", Age = 21 },
                new UserModel() { Username = "tmusthofa", FullName = "Tegar Aryo Sulthon Musthofa", Age = 32 },
                new UserModel() { Username = "nubs", FullName = "Nubli Hawari", Age = 28 },
                new UserModel() { Username = "nadiaita", FullName = "Nadia Ita Purnamasari", Age = 20 }
            };

            UserExcelModel userExcelModel = new UserExcelModel(userDatas);
            return userExcelModel.ToDataTable();
        }
    }
}
