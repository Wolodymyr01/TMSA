using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMSA.Domain
{
    public class Client : Entity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        private List<Booking> Bookings { get; set; } = new List<Booking>();
        public IReadOnlyList<Booking> GetBookings => Bookings;
        public DateTime DateOfBirth { get; private set; }
    }
}
