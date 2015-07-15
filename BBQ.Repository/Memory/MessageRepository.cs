using BBQ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.Memory
{
    public class MessageRepository : IMessageRepository
    {
        protected static List<Message> Messages = new List<Message>();

        protected Guid AccountID { get; set; }

        public bool Add(Model.Message a)
        {
            if (a == null || a.AccountID==Guid.Empty || a.QueueID== Guid.Empty) return false;

            Messages.Add(a);
            return true;
        }

        public bool Delete(Guid id)
        {
            var q = Get(id);
            if (q == null) return false;
            Messages.Remove(q);
            return true;
        }

        public Model.Message Get(Guid id)
        {
            return Messages.SingleOrDefault(q => q.AccountID == AccountID && q.ID == id);
        }

        public bool Save(Model.Message a)
        {
            var q = Get(a.ID);
            if (q == null) return false;

            Messages.Remove(q);
            Messages.Add(q);

            return true;
        }

        public bool Init(Guid AccountID, string ConnectionString="")
        {
            this.AccountID = AccountID;
            return true;
        }

        private object _locker = new object();
        public Message GetNextAndLock(Guid QueueID)
        {
            lock(_locker)
            {
                var m = Messages.Where(q => q.AccountID == AccountID && q.QueueID == QueueID && q.Status== MessageStatus.Pending)
                            .OrderBy(q=>q.TimeStamp)
                            .FirstOrDefault();
                if(m!=null)
                {
                    m.Status = MessageStatus.Locked;
                    m.StatusTimeStamp = DateTime.Now;
                    m.Attempted++;                    
                }

                return m;
            }
        }

        public decimal Count(Guid? guid = null, MessageStatus? status = null)
        {
            var msgs = Messages.Where(m => m.AccountID == AccountID);
            if(guid.HasValue)
            {
                msgs = msgs.Where(m => m.QueueID == guid.Value);
            }
            if(status.HasValue)
            {
                msgs = msgs.Where(m => m.Status == status.Value);
            }
            return msgs.Count();
        }

        public decimal AverageAge(Guid? guid = null, MessageStatus? status = null)
        {
            var msgs = Messages.Where(m => m.AccountID == AccountID);
            if (guid.HasValue)
            {
                msgs = msgs.Where(m => m.QueueID == guid.Value);
            }
            if (status.HasValue)
            {
                msgs = msgs.Where(m => m.Status == status.Value);
            }
            return (decimal)msgs.Select(m => (DateTime.Now - m.TimeStamp).TotalSeconds).DefaultIfEmpty().Average();
        }

        public bool UnlockIfLockedBeforeDateTime(Guid QID, DateTime lockTime)
        {
            var msgs = Messages.Where(m => m.AccountID == AccountID
                                        && m.QueueID == QID
                                        && m.Status == MessageStatus.Locked 
                                        && m.StatusTimeStamp < lockTime)
                                    .ToList();
            foreach(var m in msgs)
            {
                m.Status = MessageStatus.Pending;
                m.StatusTimeStamp = DateTime.Now;
            }

            return true;
        }

        public IList<Message> GetRetryExceededItems(Guid QID, int tries)
        {
            return Messages.Where(m => m.AccountID == AccountID
                                        && m.QueueID == QID
                                        && m.Attempted > tries)
                                    .ToList();
        }

        public bool MoveMessagesToQueue(IList<Message> msgs, Queue move_to_queue)
        {
            foreach(var msg in msgs)
            {
                var move_message = Messages.SingleOrDefault(m => m.AccountID == AccountID
                                                                && m.ID == msg.ID);
                if (move_message == null) return false;
                move_message.QueueID = move_to_queue.ID;
            }
            return true;
        }


        public IList<Message> GetItemsLastModifiedBefore(Guid QID, DateTime dateTime)
        {
            return Messages.Where(m => m.AccountID == AccountID
                                        && m.QueueID == QID
                                        && m.StatusTimeStamp < dateTime)
                                    .ToList();
        }
    }
}
