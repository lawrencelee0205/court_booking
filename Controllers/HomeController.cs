using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Court_booking.Models;
using Court_booking.Data;
using Microsoft.EntityFrameworkCore;

namespace Court_booking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Court_context _context;

        public IActionResult Index()
        {
            return View();
        }

        public HomeController(ILogger<HomeController> logger, Court_context context)
        {
            _logger = logger;
            _context = context;

        }

        public IActionResult Register_customer(string Name, string Password1, string Password2, string Contact, string Email)
        {
            var cus = _context.People.Where(c => c.Name == Name).FirstOrDefault();

            if (cus == null && !check_duplicate_name(Name))
            {
                if (Password1 == Password2)
                {
                    var new_cus = new People();
                    new_cus.Name = Name;
                    new_cus.Password = Password2;
                    new_cus.Contact = Contact;
                    new_cus.Role = "customer";
                    new_cus.Email = Email;

                    _context.Add(new_cus);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    Response.WriteAsync("<script>alert('Password not confirmed');window.location.href = 'Register';</script>");
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                Response.WriteAsync("<script>alert('Duplicate customer name found'); window.location.href = 'Register';</script>");
                return RedirectToAction(nameof(Index));
            }

            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Name, string Password)
        {

            var people = await _context.People.FirstOrDefaultAsync(m => m.Name == Name && m.Password == Password);

            if (people == null)
            {
                await Response.WriteAsync("<script>alert('Invalid user or password'); window.location.href = 'Index';</script>");
                return View("Index");
            }

            var role = people.Role.ToUpper();

            HttpContext.Session.SetString("Session_Role", role);
            HttpContext.Session.SetString("Session_Name", people.Name);
            HttpContext.Session.SetInt32("Session_Id", people.Id);

            switch (role)
            {
                case "ADMIN":
                    return RedirectToAction("Index", "Admin");

                case "CUSTOMER":
                    return RedirectToAction("Index", "Customer");

                default:
                    return View("Index");
            }



        }

        public IActionResult Logout()
        {

            HttpContext.Session.Clear();

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return View("Index");
        }

        private bool check_duplicate_name(string new_name)
        {
            bool duplicate = false;
            var name_list = _context.People.Select(p => p.Name).ToList();

            if(name_list.Contains(new_name))
            {
                duplicate = true;
            }



            return duplicate;

        }

    }
}
