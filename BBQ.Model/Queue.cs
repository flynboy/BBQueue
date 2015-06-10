using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Model
{
    public class Queue
    {
        public Guid ID { get; set; }
        public Guid AccountID { get; set; }

        public string Name { get; set; }
    
        /// <summary>
        /// The time to live for an item in the queue (defaults to 24 hours)
        /// after this time the item will be placed in a special dead letter queue (expiredItems)
        /// </summary>
        public long TimeToLive { get; set; }

        /// <summary>
        /// This the max amount of times this item will attempt to return before being moved to a dead queue (errorItems)
        /// defaults to 10
        /// </summary>
        public int MaxAttempts { get; set; }

        /// <summary>
        /// this is the amount of time in seconds an item will wait for confirmation it was processed before it sets itself to pending
        /// defaults to 5 minutes
        /// </summary>
        public long LockTimeout { get; set; }

        public Queue()
        {
            ID = Guid.NewGuid();
            TimeToLive = 86400; //default to 24 hours
            MaxAttempts = 10;
            LockTimeout = 300;
        }

        public Queue(Guid AccountID)
            :this()
        {
            this.AccountID = AccountID;
        }
    }
}
