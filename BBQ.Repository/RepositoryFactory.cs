using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository
{
    public static class RepositoryFactory
    {
        public static IAuthorizationRepository createAuthorizationRepository(Guid AccountID)
        {
            return null; //todo
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
