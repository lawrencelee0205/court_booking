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
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly Court_context _context;

        public CustomerController(ILogger<CustomerController> logger, Court_context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("Session_Id");
            var profile = _context.People.Where(p => p.Id == id).FirstOrDefault();

            return View(profile);
        }

        public IActionResult Update_profile()
        {
            int? id = HttpContext.Session.GetInt32("Session_Id");
            var profile = _context.People.Where(p => p.Id == id).FirstOrDefault();

            return View(profile);
        }

        public IActionResult Update(int? id, string Contact, string Email, string Password)
        {
            

                
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteBooking(int id)
        {

            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Booking));

        }
        public IActionResult Booking()
        {
            int? id = HttpContext.Session.GetInt32("Session_Id");
            DateTime now = DateTime.Now;
            
            ViewBag.MaxMessage = TempData["MaxMessage"];
            ViewBag.OverlapMessage = TempData["OverlapMessage"];

            try
            {
                var booking = _context.Booking.Where(b => b.Book_time.AddHours(1) > now && b.User_id == id).ToList();
                IList<BookingView> booking_view = new List<BookingView>();

                foreach (var b in booking)
                {
                    var customer = _context.People.Where(c => c.Id == b.User_id).FirstOrDefault();
                    var court = _context.Court.Where(c => c.Id == b.Court_id).FirstOrDefault();

                    booking_view.Add(new BookingView() { booking = b, people = customer, court_type = court.Type });
                }

                return View(booking_view);
            }
            catch(NullReferenceException)
            {
                return View();
            }
        }

        public IActionResult AddBooking()
        {
            DateTime now = DateTime.Now;
            int? id = HttpContext.Session.GetInt32("Session_Id");
            var booking_count = _context.Booking.Where(b => b.User_id == id && b.Book_time.AddHours(1)> now ).Count();
            

            if(booking_count == 2)
            {
               TempData["MaxMessage"] = "Max booking is 2";

                return RedirectToAction(nameof(Booking));
            }
            else
            {
                var Court_id = _context.Court.Where(c => c.Status == "open").ToList();

                return View(Court_id);
            }
            
        }

        public IActionResult Add(DateTime book_date_time, int user_id, int court_type)
        {
            var existing_booking = _context.Booking.Where(c => c.Court_id == court_type).ToList();           

            Booking b = new Booking();
            b.Book_time = book_date_time;
            b.User_id = user_id;
            b.Court_id = court_type;

            DateTime end_time = b.Book_time.AddMinutes(60);

            foreach(var booking in existing_booking)
            {
                DateTime booking_end_time = booking.Book_time.AddMinutes(60);

                if ( ((booking.Book_time <= book_date_time)&&(book_date_time <= booking_end_time)) || (( booking.Book_time<=end_time ) && (end_time<=booking_end_time)) )
                {
                    TempData["OverlapMessage"] = "Booking time overlap";
                    return RedirectToAction(nameof(Booking));
                }

            }

            _context.Add(b);
            _context.SaveChanges();

            return RedirectToAction(nameof(Booking));
        }


    }
}