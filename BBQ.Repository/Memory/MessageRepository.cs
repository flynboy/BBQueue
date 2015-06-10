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
    }
}
