using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Court_booking.Models;

namespace Court_booking.Data
{
    public class Court_context : DbContext
    {
        public Court_context(DbContextOptions<Court_context> options)
    : base(options)
        {
        }

        public DbSet<People> People { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Court> Court { get; set; }
    }
}
