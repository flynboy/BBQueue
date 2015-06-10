using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository
{
    public static class RepositoryFactory
    {
        public static Authorization.IAuthTokenParser getTokenParser(string scheme)
        {
            //for now just return normal basic parser
            return new Authorization.BasicTokenParser();
        }

        public static Authorization.IAuthStrategy getAuthorizationStrategy()
        {
            //return create if null
            //todo:  inject from config
            return new Authorization.AuthenticateWithCreateIfMissing();
        }

        public static IAuthorizationRepository createAuthorizationRepository(Guid AccountID)
        {
            //todo read config and create
            var repo = new Memory.AuthRepository();
            var connStr = "";

            if (!repo.Init(AccountID, connStr)) return null;
            
            return repo;
        }

        public static IQueueRepository createQueueRepository(Guid AccountID)
        {
            //todo:  read config and set
            var repo = new Memory.QueueRepository();
            var connStr = "";


            if (!repo.Init(AccountID, connStr)) return null;

            return repo;
        }

        public static IMessageRepository createMessageRepository(Guid AccountID)
        {
            //todo:  read config and set
             var repo = new Memory.MessageRepository();
            var connStr = "";


            if (!repo.Init(AccountID, connStr)) return null;

            return repo;
        }
    }
}
