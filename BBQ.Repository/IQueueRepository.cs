using BBQ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository
{
    public interface IQueueRepository : IRepository<Queue, Guid>
    {
        Queue GetByName(string Name);
        IEnumerable<Queue> GetAll();
    }
}
