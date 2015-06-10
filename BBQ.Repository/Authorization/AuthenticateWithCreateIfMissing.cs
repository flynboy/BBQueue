using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.Authorization
{
    public class AuthenticateWithCreateIfMissing : IAuthStrategy
    {
        public bool Authenticate(IAuthorizationRepository repo, Guid AccountID, string Secret)
        {
            var acct = repo.Get(AccountID);
            if (acct == null)
            {
                acct = new Model.Account()
                {
                    ID = AccountID,
                    Secret = Secret
                };
                if (!repo.Add(acct)) return false;
            }
            return acct.Secret == Secret;
        }
    }
}
