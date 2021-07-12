using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private ITmsaDbContext _dbContext;
        public BookingRepository(ITmsaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async void Add(Booking booking)
        {
            await _dbContext.Bookings.AddAsync(booking);
        }

        public IEnumerable<Booking> Get(Func<Booking, bool> predicate)
        {
            return _dbContext.Bookings.Where(predicate);
        }

        public void Update(Booking booking)
        {
            _dbContext.Bookings.Update(booking);
        }
    }
}
