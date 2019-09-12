using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belajar8.Models;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO;

namespace Belajar8.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            MimeMessage message = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            MailboxAddress from = new MailboxAddress("Hansen", "yohanzen.alexander@binus.edu");
            List<InternetAddress> to = new List<InternetAddress>()
            {
                new MailboxAddress("User", "yohanzen.alexander@gmail.com"),
                new MailboxAddress("User2", "yohanzen.alexander@gmail.com"),
                new MailboxAddress("User3", "yohanzen.alexander@binus.com"),
                new MailboxAddress("User3", "yohanzen.alexander@binus.com"),
                new MailboxAddress("Fernando Christyanto", "fernando.christyanto@binus.edu")
            };

            bodyBuilder.HtmlBody = System.IO.File.ReadAllText("D:/Belajar/NET Core/Belajar8/wwwroot/notifikasi-perkawinan-belum-selesai.html");
            bodyBuilder.Attachments.Add($"D:/Belajar/NET Core/Belajar8/wwwroot/Happy-Birthday-to-you-91.jpg");

            message.From.Add(from);
            message.To.AddRange(to);
            message.Subject = "Subject For the Weak";
            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.office365.com", 587, false);
            client.Authenticate("yohanzen.alexander@binus.edu", "01031998");

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();

            return View();
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
