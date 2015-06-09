using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBert.Model
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

        public Queue()
        {
            ID = Guid.NewGuid();
            TimeToLive = 86400; //default to 24 hours
        }

        public Queue(Guid AccountID)
            :this()
        {
            this.AccountID = AccountID;
        }
    }
}
