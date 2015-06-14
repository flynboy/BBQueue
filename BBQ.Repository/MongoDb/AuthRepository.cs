using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.MongoDb
{
    public class AuthRepository : DbConnection, IAuthorizationRepository
    {
        public bool Add(Model.Account a)
        {
            Accounts.InsertOneAsync(a).Wait();
            return true;
        }

        public bool Delete(Guid id)
        {
            var filter = Builders<Model.Account>.Filter.Eq("ID", id);
            var success =  Accounts.DeleteOneAsync(filter).Result;
            return success.DeletedCount == 1;
        }

        public Model.Account Get(Guid id)
        {
            var filter = Builders<Model.Account>.Filter.Eq("ID", id);
            return Accounts.Find(filter)
                            .FirstOrDefaultAsync()
                            .Result;
        }

        public bool Save(Model.Account a)
        {
            var acct = Get(a.ID);
            if (acct == null) return false;

            var filter = Builders<Model.Account>.Filter.Eq("ID", a.ID);
            var success = Accounts.ReplaceOneAsync(filter, a, new UpdateOptions() { IsUpsert = true }).Result;

            return success.ModifiedCount == 1;
        }
    }
}
