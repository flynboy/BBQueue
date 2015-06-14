using BBQ.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.MongoDb
{
    public class MessageRepository : DbConnection, IMessageRepository
    {
        public bool Add(Model.Message a)
        {
            if (a == null || a.AccountID==Guid.Empty || a.QueueID== Guid.Empty) return false;

            Messages.InsertOneAsync(a).Wait();
            return true;
        }

        public bool Delete(Guid id)
        {
            var filter = Builders<Model.Message>.Filter.Eq(m => m.ID, id);
            return Messages.DeleteOneAsync(filter).Result.DeletedCount == 1;
        }

        public Model.Message Get(Guid id)
        {
            var filter = Builders<Model.Message>.Filter.Eq(m => m.ID, id);
            return Messages.Find(filter).SingleOrDefaultAsync().Result;
        }

        public bool Save(Model.Message a)
        {
            var filter = Builders<Model.Message>.Filter.Eq(m => m.ID, a.ID);

            return Messages.ReplaceOneAsync(filter, a).Result.ModifiedCount == 1;
        }

        public Message GetNextAndLock(Guid QueueID)
        {
            var filter = Builders<Model.Message>.Filter.And(
                Builders<Model.Message>.Filter.Eq(m=>m.Status, MessageStatus.Pending),
                Builders<Model.Message>.Filter.Eq(m=>m.QueueID, QueueID)
            );

            var sort = Builders<Model.Message>.Sort.Ascending(m => m.TimeStamp);
            var update = Builders<Model.Message>.Update.Set(m => m.Status, MessageStatus.Locked)
                            .Set(m=>m.TimeStamp, DateTime.Now)
                            .Inc(m=>m.Attempted,1);

            return Messages.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Model.Message, Model.Message>() { Sort = sort }).Result;
        }

        public decimal Count(Guid? guid = null, MessageStatus? status = null)
        {
            var filter = Builders<Model.Message>.Filter.And();
            if(guid.HasValue)
            {
                filter = filter & Builders<Model.Message>.Filter.Eq(m => m.QueueID, guid.Value);
            }
            if(status.HasValue)
            {
                filter = filter & Builders<Model.Message>.Filter.Eq(m => m.Status, status.Value);
            }
            return Messages.CountAsync(filter).Result;
        }

        public decimal AverageAge(Guid? guid = null, MessageStatus? status = null)
        {
            var filter = Builders<Model.Message>.Filter.And();
            if (guid.HasValue)
            {
                filter = filter & Builders<Model.Message>.Filter.Eq(m => m.QueueID, guid.Value);
            }
            if (status.HasValue)
            {
                filter = filter & Builders<Model.Message>.Filter.Eq(m => m.Status, status.Value);
            }
            return 0; //todo
        }
    }
}
