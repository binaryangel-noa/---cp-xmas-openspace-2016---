using EfInjectors.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EfInjectors.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        [Authorize("GodModeOnly")]
        public IActionResult Students()
        {
            SchoolContext db = new SchoolContext();

            var students = from s in db.Students
                           select s;

            return View(students.ToList());
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
