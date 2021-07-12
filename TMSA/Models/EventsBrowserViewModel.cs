using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Models
{
    public class EventsBrowserViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; }
        public Event Event { get; set; }
    }
}
