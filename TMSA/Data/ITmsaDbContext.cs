using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data
{
    public interface ITmsaDbContext
    {
        DbSet<Event> Events { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<Booking> Bookings { get; set; }
    }
}
