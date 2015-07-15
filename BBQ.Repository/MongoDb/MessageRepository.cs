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
                            .Set(m=>m.StatusTimeStamp, DateTime.Now)
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

        public bool UnlockIfLockedBeforeDateTime(Guid QID, DateTime lockTime)
        {
            var filter = Builders<Model.Message>.Filter.And(
                Builders<Model.Message>.Filter.Eq(m => m.Status, MessageStatus.Locked),
                Builders<Model.Message>.Filter.Eq(m => m.QueueID, QID),
                Builders<Model.Message>.Filter.Lt(m=> m.StatusTimeStamp, lockTime)
            );

            var update = Builders<Model.Message>.Update.Set(m => m.Status, MessageStatus.Pending)
                            .Set(m => m.StatusTimeStamp, DateTime.Now);

            try
            {
                var res = Messages.UpdateManyAsync(filter, update).Result;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IList<Message> GetRetryExceededItems(Guid QID, int tries)
        {
            var filter = Builders<Model.Message>.Filter.And(
                Builders<Model.Message>.Filter.Eq(m => m.QueueID, QID),
                Builders<Model.Message>.Filter.Gt(m => m.Attempted, tries)
            );

            return Messages.Find(filter).ToListAsync().Result;
        }

        public bool MoveMessagesToQueue(IList<Message> msgs, Queue move_to_queue)
        {
            List<FilterDefinition<Message>> msg_filters = new List<FilterDefinition<Message>>();

            foreach (var msg in msgs)
            {
                msg_filters.Add(Builders<Model.Message>.Filter.Eq(m => m.ID, msg.ID));                                
            }

            var filter = Builders<Model.Message>.Filter.Or(msg_filters);

            var update = Builders<Model.Message>.Update.Set(m => m.QueueID, move_to_queue.ID);

            try
            {
                var res = Messages.UpdateManyAsync(filter, update).Result;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public IList<Message> GetItemsLastModifiedBefore(Guid QID, DateTime dateTime)
        {
            var filter = Builders<Model.Message>.Filter.And(
                Builders<Model.Message>.Filter.Eq(m => m.QueueID, QID),
                Builders<Model.Message>.Filter.Lt(m=> m.StatusTimeStamp, dateTime)
            );

            return Messages.Find(filter).ToListAsync().Result;
        }
    }
}
