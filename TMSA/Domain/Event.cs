using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMSA.Domain
{
    public class Event : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime EventTime { get; private set; }
    }
}
