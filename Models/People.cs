using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Court_booking.Models
{
    public class People
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
    }
}
