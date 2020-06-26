using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Court_booking.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime Book_time { get; set; }
        public int User_id { get; set; }
        public int Court_id { get; set; }
    }
}
