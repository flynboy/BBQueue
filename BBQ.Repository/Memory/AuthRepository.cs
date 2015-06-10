using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.Memory
{
    public class AuthRepository : IAuthorizationRepository
    {
        private static List<Model.Account> Accounts = new List<Model.Account>();

        private Guid AccountID { get; set; }

        public bool Init(Guid AccountID, string ConnectionString = "")
        {
            this.AccountID = AccountID;
            return true;
        }

        public bool Add(Model.Account a)
        {
            Accounts.Add(a);
            return true;
        }

        public bool Delete(Guid id)
        {
            var a = Get(id);
            if (a == null) return false;
            Accounts.Remove(a);
            return true;
        }

        public Model.Account Get(Guid id)
        {
            return Accounts.SingleOrDefault(a => a.ID == id);
        }

        public bool Save(Model.Account a)
        {
            var acct = Get(a.ID);
            if (acct == null) return false;

            Accounts.Remove(acct);
            Accounts.Add(a);
            return true;
        }
    }
}
