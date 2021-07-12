using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data.Repositories
{
    public interface IClientRepository
    {
        void Add(Client client);
        void Update(Client client);
        IEnumerable<Client> Get(Func<Client, bool> predicate);
    }
}
