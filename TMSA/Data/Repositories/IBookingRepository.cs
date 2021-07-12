using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data.Repositories
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> Get(Func<Booking, bool> predicate);
        void Add(Booking booking);
        void Update(Booking booking);
    }
}
