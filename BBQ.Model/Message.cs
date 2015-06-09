using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Model
{
    public enum MessageStatus
    {
        Pending,
        Locked,
        Expired,
        Error
    }

    public class Message
    {
        public Guid ID { get; set; }
        public Guid QueueID { get; set; }
        public Guid AccountID { get; set; }

        public DateTime TimeStamp { get; set; }
        public int Attempted { get; set; }
        public MessageStatus Status { get; set; }

        public object Payload { get; set; }

        public Message()
        {
            ID = Guid.NewGuid();
            TimeStamp = DateTime.Now;
            Attempted = 0;
            Status = MessageStatus.Pending;
        }

        public Message(Queue q)
            :this()
        {
            QueueID = q.ID;
            AccountID = q.AccountID;
        }
    }
}
