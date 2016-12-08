using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EfInjectors.Controllers
{
    public class LoginModel
    {
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class LoginController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult>  Index()
        {
            var model = new LoginModel { User = "me", Password = "some" };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel loginModel)
        {
            if (true)
            {
                try
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, loginModel.User), new Claim(ClaimTypes.Role, "God") };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    return Redirect("/");
                }
                catch (Exception ex)
                {

                    throw;
                }
               
            }
            return View("Index", loginModel);
        }
    }
}
