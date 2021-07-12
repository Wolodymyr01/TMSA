using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data.Repositories
{
    public interface IEventRepository
    {
        void Add(Event evnt);
        void Update(Event evnt);
        IEnumerable<Event> Get(Func<Event, bool> predicate);
    }
}
