using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Court_booking.Data;
using Court_booking.Models;
using System.Diagnostics;
using Court_booking.ViewModel;

namespace Court_booking.Controllers
{
    public class AdminController : Controller
    {
        const string veryrole = "ADMIN";
        private readonly ILogger<AdminController> _logger;
        private readonly Court_context _context;

        public AdminController(ILogger<AdminController> logger, Court_context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Update_profile()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                int? id = HttpContext.Session.GetInt32("Session_Id");
                var profile = _context.People.Where(p => p.Id == id).FirstOrDefault();

                return View(profile);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Update(int? id, string? Contact, string? Email, string? new_password, string? confirm_password)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var people = _context.People.Where(c => c.Id == id).FirstOrDefault();

                if (new_password != confirm_password)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    people.Contact = Contact;
                    people.Email = Email;
                    if (new_password != null)
                    {
                        people.Password = new_password;
                    }
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                int? id = HttpContext.Session.GetInt32("Session_Id");
                var profile = _context.People.Where(p => p.Id == id).FirstOrDefault();

                return View(profile);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Customer()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var cus = _context.People.Where(c => c.Role == "Customer");

                ViewData["Customer"] = cus.ToList();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DeleteHistory(int id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var history = _context.Booking.Find(id);

                _context.Booking.Remove(history);
                _context.SaveChanges();

                return RedirectToAction(nameof(History));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Court()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var court = _context.Court.ToList();

                return View(court);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var court = await _context.Court
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (court == null)
                {
                    return NotFound();
                }

                return View(court);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult Change_status(string? status, int id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var court = _context.Court.First(c => c.Id == id);

                court.Status = status;

                _context.SaveChanges();

                return RedirectToAction(nameof(Court));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Search_customer_name(string? customer_name)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                if (customer_name == null)
                {
                    return RedirectToAction(nameof(Booking));
                }

                DateTime now = DateTime.Now;

                var cus = _context.People.Where(c => c.Name.Contains(customer_name)).FirstOrDefault();
                int cus_id = cus.Id;

                try
                {
                    var booking = _context.Booking.Where(b => b.Book_time.AddHours(1) > now && b.User_id == cus_id).ToList();
                    IList<BookingView> booking_view = new List<BookingView>();

                    foreach (var b in booking)
                    {
                        var customer = _context.People.Where(c => c.Id == b.User_id).FirstOrDefault();
                        var court = _context.Court.Where(c => c.Id == b.Court_id).FirstOrDefault();

                        booking_view.Add(new BookingView() { booking = b, people = customer, court_type = court.Type });
                    }


                    return View("Booking", booking_view);
                }
                catch (NullReferenceException)
                {
                    return RedirectToAction(nameof(Booking));
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Booking()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                DateTime now = DateTime.Now;

                try
                {
                    var booking = _context.Booking.Where(b => b.Book_time.AddHours(1) > now).ToList();
                    IList<BookingView> booking_view = new List<BookingView>();

                    foreach (var b in booking)
                    {
                        var customer = _context.People.Where(c => c.Id == b.User_id).FirstOrDefault();
                        var court = _context.Court.Where(c => c.Id == b.Court_id).FirstOrDefault();

                        booking_view.Add(new BookingView() { booking = b, people = customer, court_type = court.Type });
                    }

                    Debug.WriteLine($" Booking is null : {booking == null}");



                    Debug.WriteLine("Not null reference");
                    return View(booking_view);
                }
                catch (NullReferenceException)
                {
                    Debug.WriteLine("Null reference ");


                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditCustomer(int id, string Email, string Contact)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var customer = _context.People.Where(c => c.Id == id).FirstOrDefault();

                customer.Email = Email;
                customer.Contact = Contact;

                _context.SaveChanges();

                return RedirectToAction(nameof(Customer));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.People.Where(c => c.Id == id).FirstOrDefault();

            return View(customer);
        }

        public async Task<IActionResult> DeleteCus(int id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var customer = await _context.People.FindAsync(id);
                _context.People.Remove(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Customer));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult EditCus(int id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var customer = _context.People.Where(c => c.Id == id).FirstOrDefault();

                return View(customer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult History()
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                DateTime now = DateTime.Now;

                var booking = _context.Booking.Where(b => b.Book_time.AddHours(1) < now).ToList();
                IList<BookingView> booking_view = new List<BookingView>();

                foreach (var b in booking)
                {
                    var customer = _context.People.Where(c => c.Id == b.User_id).FirstOrDefault();
                    var court = _context.Court.Where(c => c.Id == b.Court_id).FirstOrDefault();



                    booking_view.Add(new BookingView() { booking = b, people = customer, court_type = court.Type });
                }


                return View(booking_view);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> DeleteBooking(int id)
        {
            if (HttpContext.Session.GetString("Session_Role") == veryrole)
            {
                var booking = await _context.Booking.FindAsync(id);

                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Booking));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}