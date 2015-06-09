using QBert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBert.Repository
{
    public interface IAuthorizationRepository : IRepository<Account, Guid>
    {
        bool Authorize(Guid id, string Secret);
    }
}
