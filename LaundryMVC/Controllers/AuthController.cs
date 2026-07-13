using LaundryMVC.Models;
using LaundryMVC.Repository.IRepo;
using LaundryMVC.Repository.Repo;
using Microsoft.AspNetCore.Mvc;

namespace LaundryMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthIRepo _auth;

        public AuthController(IAuthIRepo auth)
        {
            _auth = auth;
        }
     
        public IActionResult AdminRegister()
        {
            return View();
        }

   
        [HttpPost]
        public async Task<IActionResult> AdminRegister(Register reg)
        {
            reg.role = "Admin";

            var result = await _auth.AdminRegister(reg);

            if (!result)
            {
                ViewBag.Error = "Admin Registration Failed";
                return View();
            }
            Login login = new Login()
            {
                Email = reg.Email,
                Password = reg.Password
            };

            var token = await _auth.AdminLogin(login);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("AdminLogin");
            }
            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("Role", "Admin");
            HttpContext.Session.SetString("UserEmail", reg.Email);
            return RedirectToAction("Dashboard", "Admin");
        }
        public IActionResult CustomerRegister()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerRegister(Register reg)
        {
            reg.role = "Customer";

            var result =
                await _auth.CustomerRegister(reg);

            if (!result)
            {
                ViewBag.Error = "Registration Failed";
                return View();
            }
            Login login = new Login()
            {
                Email = reg.Email,
                Password = reg.Password
            };

            var token =await _auth.CustomerLogin(login);

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("CustomerLogin","Auth");
            }

            HttpContext.Session.SetString("JWToken",token);
            HttpContext.Session.SetString("UserId","1");
            // STORE EMAIL
            HttpContext.Session.SetString("UserEmail",reg.Email);
            Console.WriteLine(" Auto Login After Register Success");
            return RedirectToAction("Index","Laun");
        }
        // CUSTOMER LOGIN PAGE
        public IActionResult CustomerLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CustomerLogin(Login login)
        {
            var token = await _auth.CustomerLogin(login);

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error ="Invalid Login";
                return View();
            }

            HttpContext.Session.SetString("JWToken",token);
            HttpContext.Session.SetString("UserId","1");

            //  ADD THIS
            HttpContext.Session.SetString("UserEmail",login.Email);

            return RedirectToAction("Index","Laun");
        }
         
        
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(Login model)
        {
            var token = await _auth.AdminLogin(model);

            if (token != null)
            {
            
                HttpContext.Session.SetString("JWToken", token);
                HttpContext.Session.SetString("Role", "Admin");

                return RedirectToAction("Dashboard", "Admin");
            }

            ViewBag.Error = "Invalid Login";
            return View();
        }
    }
}
