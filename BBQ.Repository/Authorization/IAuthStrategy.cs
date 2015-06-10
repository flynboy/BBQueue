using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.Authorization
{
    public interface IAuthStrategy
    {
        bool Authenticate(IAuthorizationRepository repo, Guid AccountID, string Secret);
    }
}
