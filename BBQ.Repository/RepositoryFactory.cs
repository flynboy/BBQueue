using System;
using System.Collections.Generic;
using System.Configuration;
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

        public static string getRepoSetting(string subType)
        {
            var baseVal = ConfigurationManager.AppSettings["BBQ.Repository"] ?? "";
            var subVal = ConfigurationManager.AppSettings["BBQ.Repository." + subType] ?? "";

            return string.IsNullOrEmpty(subVal) ? baseVal : subVal;
        }

        public static string getRepoConectionString(string subType)
        {
            var baseVal = ConfigurationManager.AppSettings["BBQ.Repository.ConnectionString"] ?? "";
            var subVal = ConfigurationManager.AppSettings["BBQ.Repository." + subType + ".ConnectionString"] ?? "";

            return string.IsNullOrEmpty(subVal) ? baseVal : subVal;
        }

        public static IAuthorizationRepository createAuthorizationRepository(Guid AccountID)
        {
            IAuthorizationRepository repo;

            switch(getRepoSetting("Authorization").ToLower())
            {
                case "mongodb":
                    repo = new MongoDb.AuthRepository();
                    break;
                default:
                    repo = new Memory.AuthRepository();
                    break;
            }
            
            if(repo==null) return null;

            if (!repo.Init(AccountID, getRepoConectionString("Authorization"))) return null;
            
            return repo;
        }

        public static IQueueRepository createQueueRepository(Guid AccountID)
        {
            IQueueRepository repo;

            switch (getRepoSetting("Queue").ToLower())
            {
                case "mongodb":
                    repo = new MongoDb.QueueRepository();
                    break;
                default:
                    repo = new Memory.QueueRepository();
                    break;
            }
            
            if(repo==null) return null;

            if (!repo.Init(AccountID, getRepoConectionString("Queue"))) return null;
            
            return repo;
        }

        public static IMessageRepository createMessageRepository(Guid AccountID)
        {
            IMessageRepository repo;

            switch (getRepoSetting("Message").ToLower())
            {
                case "mongodb":
                    repo = new MongoDb.MessageRepository();
                    break;
                default:
                    repo = new Memory.MessageRepository();
                    break;
            }

            if (repo == null) return null;

            if (!repo.Init(AccountID, getRepoConectionString("Message"))) return null;

            return repo;
        }
    }
}
