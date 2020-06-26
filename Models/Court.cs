using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Court_booking.Models
{
    public class Court
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
