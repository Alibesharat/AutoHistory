using AutoHistory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Data;
using Sample.Models;
using System.Diagnostics;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyContext _db;

        public HomeController(ILogger<HomeController> logger, MyContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index(int Id)
        {
            Student st = new Student()
            {
                Name = "Ali",
                Id = Id
            };
            _db.Students.Add(st);
            _db.SaveChangesWithHistory();
            return Json(st.Hs_Change);
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
