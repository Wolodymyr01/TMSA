using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private ITmsaDbContext _dbContext;
        public EventRepository(ITmsaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async void Add(Event evnt)
        {
            await _dbContext.Events.AddAsync(evnt);
        }

        public IEnumerable<Event> Get(Func<Event, bool> predicate)
        {
            return _dbContext.Events.Where(predicate);
        }

        public void Update(Event evnt)
        {
            _dbContext.Events.Update(evnt);
        }
    }
}
