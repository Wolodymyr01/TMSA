using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMSA.Domain
{
    public class Booking : Entity
    {
        public Event Event { get; private set; }
        public Client Client { get; internal set; }
        public uint SeatNumber { get; private set; }
    }
}
