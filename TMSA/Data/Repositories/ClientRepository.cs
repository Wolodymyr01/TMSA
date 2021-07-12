using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSA.Domain;

namespace TMSA.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private ITmsaDbContext _dbContext;
        public ClientRepository(ITmsaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async void Add(Client client)
        {
            await _dbContext.Clients.AddAsync(client);
        }

        public IEnumerable<Client> Get(Func<Client, bool> predicate)
        {
            return _dbContext.Clients.Where(predicate);
        }

        public void Update(Client client)
        {
            _dbContext.Clients.Update(client);
        }
    }
}
