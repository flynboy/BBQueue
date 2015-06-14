using BBQ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BBQ.Repository.MongoDb
{
    public class QueueRepository : DbConnection, IQueueRepository
    {
        public bool Add(Model.Queue a)
        {
            if (a == null || a.AccountID==Guid.Empty) return false;

            Queues.InsertOneAsync(a).Wait();
            return true;
        }

        public bool Delete(Guid id)
        {
            var filter = Builders<Model.Queue>.Filter.Eq("ID", id);
            return Queues.DeleteOneAsync(filter).Result.DeletedCount == 1;
        }

        public Model.Queue Get(Guid id)
        {
            var bldr = Builders<Model.Queue>.Filter;
            var filter = bldr.Eq(q=>q.ID, id);

            return Queues.Find(filter).SingleOrDefaultAsync().Result;
        }

        public bool Save(Model.Queue a)
        {
            var filter = Builders<Model.Queue>.Filter.Eq("ID", a.ID);
            return Queues.ReplaceOneAsync(filter, a, new UpdateOptions() { IsUpsert = true }).Result.ModifiedCount == 1;
        }

        public Queue GetByName(string Name)
        {
            var filter = Builders<Model.Queue>.Filter.Eq("Name", Name);
            return Queues.Find(filter).SingleOrDefaultAsync().Result;
        }

        public IEnumerable<Queue> GetAll()
        {
            return Queues.Find(new BsonDocument()).ToListAsync().Result;
        }

        public decimal Count
        {
            get
            {
                return Queues.CountAsync(new BsonDocument()).Result;
            }
        }
    }
}
